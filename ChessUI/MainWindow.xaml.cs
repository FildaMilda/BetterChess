using ChessLogic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChessUI
{
    /// <summary>
    /// Main window class for the chess application. Handles the board initialization, input, and game logic.
    /// </summary>
    public partial class MainWindow : Window
    {
        // 2D arrays to store images of pieces and highlights on the chessboard
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highlights = new Rectangle[8, 8];
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();

        private GameState gameState;
        private Position selectedPos = null; // Currently selected position on the board

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            gameState = new GameState(Player.White, Board.Initial());

            // Example test setups for different game scenarios
            // gameState = new GameState(Player.White, Board.InitialCastleTest());
            // gameState = new GameState(Player.White, Board.InitialEnPassantTest());
            // gameState = new GameState(Player.White, Board.InitialPromotionTest());
            // gameState = new GameState(Player.White, Board.InitialEndingTest());

            // Draw the initial board and set cursor for the current player
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
        }

        // Initializes the visual board by creating images and highlight rectangles for each square
        private void InitializeBoard()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Image image = new Image();
                    pieceImages[r, c] = image;
                    PieceGrid.Children.Add(image); // Add piece images to the grid

                    Rectangle highlight = new Rectangle();
                    highlights[r, c] = highlight;
                    HighlightGrid.Children.Add(highlight); // Add highlight rectangles to the grid
                }
            }
        }

        // Draws the board by setting each square's image source to the corresponding chess piece
        private void DrawBoard(Board board)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Piece piece = board[r, c];
                    pieceImages[r, c].Source = Images.GetImage(piece); // Set image for each piece
                }
            }
        }

        // Handles mouse clicks on the board
        private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsMenuOnScreen())
            {
                return; // Ignore if any menu is currently displayed
            }

            Point point = e.GetPosition(BoardGrid);
            Position pos = ToSquarePosition(point);

            if (selectedPos == null)
            {
                OnFromPositionSelected(pos); // Select the piece to move
            }
            else
            {
                OnToPositionSelected(pos); // Select the destination square
            }
        }

        // Converts a clicked point to a board position
        private Position ToSquarePosition(Point point)
        {
            double squareSize = BoardGrid.ActualWidth / 8;
            int row = (int)(point.Y / squareSize);
            int col = (int)(point.X / squareSize);
            return new Position(row, col);
        }

        // Handles the selection of the starting position of a piece
        private void OnFromPositionSelected(Position pos)
        {
            IEnumerable<Move> moves = gameState.LegalMovesForPiece(pos);

            if (moves.Any())
            {
                selectedPos = pos;
                CacheMoves(moves); // Cache legal moves for highlighting
                ShowHighlights(); // Highlight legal destination squares
            }
        }

        // Handles the selection of the destination square for a piece
        private void OnToPositionSelected(Position pos)
        {
            selectedPos = null;
            HideHighlights(); // Hide the highlights after selection

            if (moveCache.TryGetValue(pos, out Move move))
            {
                if (move.Type == MoveType.PawnPromotion)
                {
                    HandlePromotion(move.FromPos, move.ToPos); // Handle pawn promotion
                }
                else
                {
                    HandleMove(move); // Make the move
                }
            }
        }

        // Handles pawn promotion, showing the promotion menu
        private void HandlePromotion(Position from, Position to)
        {
            pieceImages[to.Row, to.Column].Source = Images.GetImage(gameState.CurrentPlayer, PieceType.Pawn);
            pieceImages[from.Row, from.Column].Source = null;

            PromotionMenu promMenu = new PromotionMenu(gameState.CurrentPlayer);
            MenuContainer.Content = promMenu; // Show promotion menu

            promMenu.PieceSelected += type =>
            {
                MenuContainer.Content = null;
                Move promMove = new PawnPromotion(from, to, type);
                HandleMove(promMove); // Handle the promotion move
            };
        }

        // Handles making a move and updating the board
        private void HandleMove(Move move)
        {
            gameState.MakeMove(move);
            DrawBoard(gameState.Board); // Redraw the board after the move
            SetCursor(gameState.CurrentPlayer); // Update the cursor based on the current player

            if (gameState.IsGameOver())
            {
                ShowGameOver(); // Show game over screen if the game has ended
            }
        }

        // Sets the cursor based on the current player's color
        private void SetCursor(Player player)
        {
            if (player == Player.White)
            {
                Cursor = ChessCursors.WhiteCursor;
            }
            else
            {
                Cursor = ChessCursors.BlackCursor;
            }
        }

        // Caches legal moves for the selected piece
        private void CacheMoves(IEnumerable<Move> moves)
        {
            moveCache.Clear();

            foreach (Move move in moves)
            {
                moveCache[move.ToPos] = move;
            }
        }

        // Highlights legal move squares for the selected piece
        private void ShowHighlights()
        {
            Color color = Color.FromArgb(150, 125, 255, 125); // Semi-transparent highlight color

            foreach (Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = new SolidColorBrush(color);
            }
        }

        // Removes the highlights from the board
        private void HideHighlights()
        {
            foreach (Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = Brushes.Transparent;
            }
        }

        // Checks if any menu is currently on screen
        private bool IsMenuOnScreen()
        {
            return MenuContainer.Content != null;
        }

        // Restarts the game and resets the board
        private void RestartGame()
        {
            selectedPos = null;
            HideHighlights();
            moveCache.Clear();
            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
        }

        // Shows the game over screen
        private void ShowGameOver()
        {
            GameOverMenu gameOverMenu = new GameOverMenu(gameState);
            MenuContainer.Content = gameOverMenu;

            gameOverMenu.OptionSelected += option =>
            {
                if (option == Option.Restart)
                {
                    MenuContainer.Content = null;
                    RestartGame(); // Restart the game
                }
                else
                {
                    Application.Current.Shutdown(); // Close the application
                }
            };
        }

        // Handles keyboard input for pausing or restarting the game
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsMenuOnScreen() && e.Key == Key.Escape)
            {
                ShowPauseMenu(); // Show pause menu on escape key press
            }
        }

        // Shows the pause menu
        private void ShowPauseMenu()
        {
            PauseMenu pauseMenu = new PauseMenu();
            MenuContainer.Content = pauseMenu;

            pauseMenu.OptionSelected += option =>
            {
                MenuContainer.Content = null;

                if (option == Option.Restart)
                {
                    RestartGame(); // Restart game from pause menu
                }
            };
        }
    }
}
