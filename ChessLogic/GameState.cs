namespace ChessLogic
{
    public class GameState
    {
        public Board Board { get; }  // Represents the current chess board.
        public Result Result { get; private set; } = null;  // Stores the game result (null if game is ongoing).
        public Player CurrentPlayer { get; private set; }  // Tracks the current player's turn.

        private int noCaptureOrPawnMoves = 0;  // Tracks moves made without captures or pawn moves (for the 50-move rule).
        private string stateString;  // Stores the current board state in FEN format.

        private readonly Dictionary<string, int> stateHistory = new Dictionary<string, int>();  // Tracks the occurrence of each board state for threefold repetition.

        // Initializes the game state with the starting player and board.
        public GameState(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;

            stateString = new StateString(CurrentPlayer, board).ToString();  // Converts the current state to FEN.
            stateHistory[stateString] = 1;  // Adds the state to the history.
        }

        // Initializes the game state from a FEN string.
        public GameState(string FEN)
        {
            string[] sections = FEN.Split(' ');

            Board = Board.Initial(FEN);  // Initializes the board from FEN string.
            CurrentPlayer = (sections[1] == "w") ? Player.White : Player.Black;  // Sets current player from FEN.

            stateString = new StateString(CurrentPlayer, Board).ToString();  // Sets the initial state string.
            stateHistory[stateString] = 1;  // Records the initial state in the history.
        }

        // Converts chess notation (e.g., "E2") to a Position object.
        public Position getPosition(string chessCoor)
        {
            char file = chessCoor[0];
            int rank = int.Parse(chessCoor[1].ToString());
            int column = file - 'A';  // Convert file (letter) to column index.
            int row = rank - 1;  // Convert rank to row index.
            return new Position(row, column);
        }

        // Updates the FEN state string and tracks state history for threefold repetition.
        private void UpdateStateString()
        {
            stateString = new StateString(CurrentPlayer, Board).ToString();  // Generate current state string.

            if (!stateHistory.ContainsKey(stateString))
            {
                stateHistory[stateString] = 1;  // First occurrence of this state.
            }
            else
            {
                stateHistory[stateString]++;  // Increment occurrence count for this state.
            }
        }

        // Checks if the current board state has occurred three times.
        private bool ThreefoldRepetition()
        {
            return stateHistory[stateString] == 3;  // True if this state has occurred 3 times.
        }

        // Returns all legal moves for a piece at the given position.
        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if (Board.IsEmpty(pos) || Board[pos].Color != CurrentPlayer)
            {
                return Enumerable.Empty<Move>();  // No legal moves if position is empty or opponent's piece.
            }

            Piece piece = Board[pos];
            IEnumerable<Move> moveCandidates = piece.GetMoves(pos, Board);  // Get candidate moves for the piece.
            return moveCandidates.Where(move => move.IsLegal(Board));  // Return only legal moves.
        }

        // Executes a move and updates the game state.
        public void MakeMove(Move move)
        {
            Board.SetPawnSkipPosition(CurrentPlayer, null);  // Reset en passant possibility.
            bool captureOrPawn = move.Execute(Board);  // Execute move, return true if it's a capture or pawn move.

            if (captureOrPawn)
            {
                noCaptureOrPawnMoves = 0;  // Reset counter for 50-move rule.
                stateHistory.Clear();  // Clear state history on capture or pawn move.
            }
            else
            {
                noCaptureOrPawnMoves++;  // Increment no-capture/pawn-move counter.
            }
            CurrentPlayer = CurrentPlayer.Opponent();  // Switch turns.
            UpdateStateString();  // Update the board state.
            CheckForGameOver();  // Check if the game is over.
        }

        // Returns all legal moves for a specific player.
        public IEnumerable<Move> AllLegalMovesFor(Player player)
        {
            IEnumerable<Move> moveCandidates = Board.PiecePositionsFor(player).SelectMany(pos =>
            {
                Piece piece = Board[pos];
                return piece.GetMoves(pos, Board);  // Get moves for each piece.
            });

            return moveCandidates.Where(move => move.IsLegal(Board));  // Return only legal moves.
        }

        // Checks if the game has ended, updates the result if it has.
        private void CheckForGameOver()
        {
            if (!AllLegalMovesFor(CurrentPlayer).Any())  // No legal moves.
            {
                if (Board.IsInCheck(CurrentPlayer))
                {
                    Result = Result.Win(CurrentPlayer.Opponent());  // Current player is in checkmate.
                }
                else
                {
                    Result = Result.Draw(EndReason.Stalemate);  // Stalemate (no legal moves, not in check).
                }
            }
            else if (Board.InsufficientMaterial())  // Insufficient material to checkmate.
            {
                Result = Result.Draw(EndReason.InsufficientMaterial);
            }
            else if (FiftyMoveRule())  // 50 moves without a capture or pawn move.
            {
                Result = Result.Draw(EndReason.FiftyMoveRule);
            }
            else if (ThreefoldRepetition())  // Threefold repetition.
            {
                Result = Result.Draw(EndReason.ThreefoldRepetition);
            }
        }

        // Returns whether the game has ended.
        public bool IsGameOver()
        {
            return Result != null;
        }

        // Checks if the 50-move rule applies.
        private bool FiftyMoveRule()
        {
            int fullMoves = noCaptureOrPawnMoves / 2;  // Count full moves (each player moving).
            return fullMoves == 50;  // 50 full moves triggers the rule.
        }
    }
}
