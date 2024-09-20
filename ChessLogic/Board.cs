using System.Runtime.CompilerServices;

namespace ChessLogic
{
    public class Board
    {
        private readonly Piece[,] pieces = new Piece[8, 8];

        private readonly Dictionary<Player, Position> pawnSkipPositions = new Dictionary<Player, Position>
        {
            { Player.White, null },
            { Player.Black, null }
        };

        // Indexer to get or set a piece at a specific row and column.
        public Piece this[int row, int col]
        {
            get { return pieces[row, col]; }
            set { pieces[row, col] = value; }
        }

        // Indexer to get or set a piece at a specific Position object.
        public Piece this[Position pos]
        {
            get { return this[pos.Row, pos.Column]; }
            set { this[pos.Row, pos.Column] = value; }
        }

        // Initializes the board to the standard starting position.
        public static Board Initial()
        {
            Board board = new Board();
            board.FromString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            return board;
        }

        // Initializes the board from a given FEN string.
        public static Board Initial(string FEN)
        {
            Board board = new Board();
            board.FromString(FEN);  // Set board pieces based on FEN.
            board.UpdateHasMoved();  // Update pieces' "HasMoved" status.
            return board;
        }

        // Marks all pieces on the board as having moved.
        private void UpdateHasMoved()
        {
            foreach (Position pos in PiecePositions())  // Loop through all piece positions.
            {
                this[pos].HasMoved = true;  // Set the "HasMoved" flag for each piece.
            }
        }

        // Creates and returns a deep copy of the board.
        public Board Copy()
        {
            Board copy = new Board();

            foreach (Position pos in PiecePositions())  // Loop through all piece positions.
            {
                copy[pos] = this[pos].Copy();  // Copy each piece to the new board.
            }

            return copy;
        }

        // Loads the board state from a FEN string.
        public void FromString(string FEN)
        {
            string[] sections = FEN.Split(' ');  // Split FEN into its sections.
            string piecePlacement = sections[0];  // Get piece placement.
            string[] rows = piecePlacement.Split('/');  // Split rows of the board.

            for (int row = 0; row < 8; row++)
            {
                int col = 0;
                foreach (char c in rows[row])
                {
                    if (char.IsDigit(c))
                    {
                        col += (int)char.GetNumericValue(c);  // Skip empty squares.
                    }
                    else
                    {
                        Player player = char.IsUpper(c) ? Player.White : Player.Black;  // Determine piece color.
                        Piece piece = CreatePiece(c, player);  // Create piece based on character.
                        this[row, col] = piece;  // Place piece on the board.
                        col++;
                    }
                }
            }
        }

        // Creates a piece object based on the character (from FEN).
        private Piece CreatePiece(char c, Player player)
        {
            return char.ToLower(c) switch
            {
                'r' => new Rook(player),
                'n' => new Knight(player),
                'b' => new Bishop(player),
                'q' => new Queen(player),
                'k' => new King(player),
                'p' => new Pawn(player),
                _ => null,  // If the character doesn't match a piece, return null.
            };
        }


        public static string ConvertMove(Move move)
        {
            if (move == null) return "null";

            string from = PositionToAlgebraic(move.FromPos);
            string to = PositionToAlgebraic(move.ToPos);
            return from + to;
        }

        static string PositionToAlgebraic(Position pos)
        {
            char file = (char)('a' + pos.Column);
            int rank = 8 - pos.Row;
            return $"{file}{rank}";
        }

        // Initializes a board setup for testing castling.
        public static Board InitialCastleTest()
        {
            Board board = new Board();
            board.AddCastleTestPieces();  // Add pieces in positions relevant for castling tests.
            return board;
        }

        // Adds pieces to the board for castling tests (Kings and Rooks).
        private void AddCastleTestPieces()
        {
            this[0, 0] = new Rook(Player.Black);  // Black Rook on a8.
            this[0, 7] = new Rook(Player.Black);  // Black Rook on h8.
            this[7, 0] = new Rook(Player.White);  // White Rook on a1.
            this[7, 7] = new Rook(Player.White);  // White Rook on h1.

            this[0, 4] = new King(Player.Black);  // Black King on e8.
            this[7, 4] = new King(Player.White);  // White King on e1.

            this[0, 2] = new Bishop(Player.Black);  // Black Bishop on c8 (to block castling).
            this[7, 5] = new Bishop(Player.White);  // White Bishop on f1 (to block castling).
        }

        // Initializes a board setup for testing en passant.
        public static Board InitialEnPassantTest()
        {
            Board board = new Board();
            board.AddEnPassantTestPieces();  // Add pieces for en passant test.
            return board;
        }

        // Adds pieces to the board for en passant tests.
        private void AddEnPassantTestPieces()
        {
            this[0, 4] = new King(Player.Black);  // Black King on e8.
            this[7, 4] = new King(Player.White);  // White King on e1.

            this[0, 3] = new Queen(Player.Black);  // Black Queen on d8.
            this[7, 3] = new Queen(Player.White);  // White Queen on d1.

            this[4, 3] = new Pawn(Player.Black);  // Black Pawn on d5 (positioned for en passant capture).
            this[6, 4] = new Pawn(Player.White);  // White Pawn on e2 (positioned for en passant test).
        }

        // Initializes a board setup for testing promotion.
        public static Board InitialPromotionTest()
        {
            Board board = new Board();
            board.AddPromotionTestPieces();  // Add pieces for promotion test.
            return board;
        }

        // Adds pieces to the board for promotion tests.
        private void AddPromotionTestPieces()
        {
            this[0, 4] = new King(Player.Black);  // Black King on e8.
            this[7, 4] = new King(Player.White);  // White King on e1.

            this[0, 3] = new Queen(Player.Black);  // Black Queen on d8.
            this[7, 3] = new Queen(Player.White);  // White Queen on d1.

            this[6, 0] = new Pawn(Player.Black);  // Black Pawn on a2 (ready for promotion).
            this[1, 7] = new Pawn(Player.White);  // White Pawn on h7 (ready for promotion).
        }

        // Initializes a board setup for testing endgame scenarios.
        public static Board InitialEndingTest()
        {
            Board board = new Board();
            board.AddEndingTestPieces();  // Add pieces for an endgame test.
            return board;
        }

        // Adds pieces to the board for endgame tests.
        private void AddEndingTestPieces()
        {
            this[0, 0] = new King(Player.Black);  // Black King on a8.
            this[7, 4] = new King(Player.White);  // White King on e1.

            this[1, 7] = new Queen(Player.White);  // White Queen on h7.
            this[7, 1] = new Rook(Player.White);  // White Rook on b1 (endgame test setup).
        }

        // Returns the position where a pawn can be captured via en passant for the specified player.
        public Position GetPawnSkipPosition(Player player)
        {
            return pawnSkipPositions[player];
        }

        // Sets the position where a pawn can be captured via en passant for the specified player.
        public void SetPawnSkipPosition(Player player, Position pos)
        {
            pawnSkipPositions[player] = pos;
        }

        // Checks if the given position is within the bounds of the chess board (0-7 for both row and column).
        public static bool IsInside(Position pos)
        {
            return pos.Row >= 0 && pos.Row < 8 && pos.Column >= 0 && pos.Column < 8;
        }

        // Checks if a given position on the board is empty (i.e., no piece is present).
        public bool IsEmpty(Position pos)
        {
            return this[pos] == null;
        }

        // Returns all positions on the board that contain a piece (non-empty squares).
        public IEnumerable<Position> PiecePositions()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Position pos = new Position(r, c);

                    if (!IsEmpty(pos))  // If the position contains a piece, yield the position.
                    {
                        yield return pos;
                    }
                }
            }
        }

        // Returns all positions that contain pieces belonging to the specified player.
        public IEnumerable<Position> PiecePositionsFor(Player player)
        {
            return PiecePositions().Where(pos => this[pos].Color == player);  // Filter positions for the given player's pieces.
        }

        // Checks if the specified player is in check by seeing if any opponent's piece can capture the player's king.
        public bool IsInCheck(Player player)
        {
            return PiecePositionsFor(player.Opponent()).Any(pos =>
            {
                Piece piece = this[pos];
                return piece.CanCaptureOpponentKing(pos, this);  // Check if any piece can capture the opponent's king.
            });
        }

        // Counts all pieces on the board by color and type.
        public Counting CountPieces()
        {
            Counting counting = new Counting();

            foreach (Position pos in PiecePositions())  // For each position that has a piece.
            {
                Piece piece = this[pos];
                counting.Increment(piece.Color, piece.Type);  // Increment the count for the piece's type and color.
            }

            return counting;
        }

        // Checks if there is insufficient material to continue the game (i.e., a draw scenario).
        public bool InsufficientMaterial()
        {
            Counting counting = CountPieces();  // Count the pieces on the board.

            // Check for different insufficient material conditions.
            return IsKingVKing(counting) || IsKingBishopVKing(counting) || IsKingKnightVKing(counting) || IsKingBishopVKingBishop(counting);
        }

        // Checks if both sides only have their kings left on the board (draw condition).
        private static bool IsKingVKing(Counting counting)
        {
            return counting.TotalCount == 2;  // Only two pieces (the kings) are present.
        }

        // Checks if the board is in a King vs. King + Bishop scenario (draw condition).
        private static bool IsKingBishopVKing(Counting counting)
        {
            return counting.TotalCount == 3 && (counting.White(PieceType.Bishop) == 1 || counting.Black(PieceType.Bishop) == 1);  // One Bishop and two Kings.
        }

        // Checks if the board is in a King vs. King + Knight scenario (draw condition).
        private static bool IsKingKnightVKing(Counting counting)
        {
            return counting.TotalCount == 3 && (counting.White(PieceType.Knight) == 1 || counting.Black(PieceType.Knight) == 1);  // One Knight and two Kings.
        }

        // Checks if the board is in a King + Bishop vs. King + Bishop scenario where both bishops are on the same color squares (draw condition).
        private bool IsKingBishopVKingBishop(Counting counting)
        {
            if (counting.TotalCount != 4)  // There must be exactly four pieces (two kings and two bishops).
            {
                return false;
            }

            if (counting.White(PieceType.Bishop) != 1 || counting.Black(PieceType.Bishop) != 1)  // Ensure each side has one Bishop.
            {
                return false;
            }

            Position wBishopPos = FindPiece(Player.White, PieceType.Bishop);  // Find White's Bishop.
            Position bBishopPos = FindPiece(Player.Black, PieceType.Bishop);  // Find Black's Bishop.

            return wBishopPos.SquareColor() == bBishopPos.SquareColor();  // Check if both bishops are on the same color square.
        }


        // Finds the position of a specific piece (type and color) on the board.
        private Position FindPiece(Player color, PieceType type)
        {
            return PiecePositionsFor(color).First(pos => this[pos].Type == type);  // Returns the first matching piece's position.
        }

        // Checks if both the King and the Rook at the given positions have not moved (required for castling).
        private bool IsUnmovedKingAndRook(Position kingPos, Position rookPos)
        {
            if (IsEmpty(kingPos) || IsEmpty(rookPos))  // Ensure both positions are occupied.
            {
                return false;
            }

            Piece king = this[kingPos];
            Piece rook = this[rookPos];

            // Return true if both are the correct type and neither has moved.
            return king.Type == PieceType.King && rook.Type == PieceType.Rook &&
                   !king.HasMoved && !rook.HasMoved;
        }

        // Checks if the player has the right to castle on the king-side (short castling).
        public bool CastleRightKS(Player player)
        {
            return player switch
            {
                Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 7)),  // White: King on e1, Rook on h1.
                Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 7)),  // Black: King on e8, Rook on h8.
                _ => false
            };
        }

        // Checks if the player has the right to castle on the queen-side (long castling).
        public bool CastleRightQS(Player player)
        {
            return player switch
            {
                Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 0)),  // White: King on e1, Rook on a1.
                Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 0)),  // Black: King on e8, Rook on a8.
                _ => false
            };
        }

        // Checks if the player can capture an opponent's pawn en passant.
        public bool CanCaptureEnPassant(Player player)
        {
            Position skipPos = GetPawnSkipPosition(player.Opponent());  // Get the en passant target position.

            if (skipPos == null)
            {
                return false;
            }

            // Determine which pawns could potentially capture en passant based on the player's color.
            Position[] pawnPositions = player switch
            {
                Player.White => new Position[] { skipPos + Direction.SouthWest, skipPos + Direction.SouthEast },
                Player.Black => new Position[] { skipPos + Direction.NorthWest, skipPos + Direction.NorthEast },
                _ => Array.Empty<Position>()
            };

            // Check if there is a pawn in position to capture en passant.
            return HasPawnInPosition(player, pawnPositions, skipPos);
        }

        // Checks if the player's pawn is in the correct position to perform en passant capture.
        private bool HasPawnInPosition(Player player, Position[] pawnPositions, Position skipPos)
        {
            foreach (Position pos in pawnPositions.Where(IsInside))  // Ensure the positions are inside the board bounds.
            {
                Piece piece = this[pos];
                // Ensure the piece is a pawn of the correct color.
                if (piece == null || piece.Color != player || piece.Type != PieceType.Pawn)
                {
                    continue;
                }

                // Create the en passant move and check if it's legal.
                EnPassant move = new EnPassant(pos, skipPos);
                if (move.IsLegal(this))  // Return true if the en passant move is legal.
                {
                    return true;
                }
            }

            return false;  // No legal en passant move found.
        }
    }
}