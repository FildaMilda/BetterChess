using System.Text;

namespace ChessLogic
{
    public class StateString
    {
        // StringBuilder used to construct the final chess state string.
        private readonly StringBuilder sb = new StringBuilder();

        // Constructor takes the current player and board and builds the state string.
        public StateString(Player currentPlayer, Board board)
        {
            AddPiecePlacement(board);      // Adds the board's piece placement in FEN format.
            sb.Append(' ');
            AddCurrentPlayer(currentPlayer); // Adds the current player's turn ('w' or 'b').
            sb.Append(' ');
            AddCastlingRights(board);      // Adds castling rights ('KQkq' or '-').
            sb.Append(' ');
            AddEnPassant(board, currentPlayer); // Adds the en passant target square or '-'.
        }

        // Overrides the ToString method to return the constructed state string.
        public override string ToString()
        {
            return sb.ToString();
        }

        // Converts a piece to its corresponding character (FEN notation).
        private static char PieceChar(Piece piece)
        {
            // Map PieceType to the corresponding character.
            char c = piece.Type switch
            {
                PieceType.Pawn => 'p',
                PieceType.Knight => 'n',
                PieceType.Rook => 'r',
                PieceType.Bishop => 'b',
                PieceType.Queen => 'q',
                PieceType.King => 'k',
                _ => ' '
            };

            // Capitalize for white pieces.
            if (piece.Color == Player.White)
            {
                return char.ToUpper(c);
            }

            return c;
        }

        // Adds FEN row data for a single row, considering empty spaces.
        private void AddRowData(Board board, int row)
        {
            int empty = 0;

            // Iterate over each column in the row.
            for (int c = 0; c < 8; c++)
            {
                if (board[row, c] == null) // Count empty spaces.
                {
                    empty++;
                    continue;
                }

                if (empty > 0) // If there were empty spaces, append their count.
                {
                    sb.Append(empty);
                    empty = 0;
                }

                sb.Append(PieceChar(board[row, c])); // Add the piece character.
            }

            if (empty > 0) // Add any trailing empty spaces.
            {
                sb.Append(empty);
            }
        }

        // Adds the piece placement for all rows in FEN format.
        private void AddPiecePlacement(Board board)
        {
            for (int r = 0; r < 8; r++)
            {
                if (r != 0)
                {
                    sb.Append('/'); // Separate rows with a '/'.
                }

                AddRowData(board, r); // Add each row's data.
            }
        }

        // Adds the current player ('w' for White, 'b' for Black).
        private void AddCurrentPlayer(Player currentPlayer)
        {
            if (currentPlayer == Player.White)
            {
                sb.Append('w');
            }
            else
            {
                sb.Append('b');
            }
        }

        // Adds castling rights to the state string (KQkq or '-') in FEN format.
        private void AddCastlingRights(Board board)
        {
            bool castleWKS = board.CastleRightKS(Player.White); // White King-side castling.
            bool castleWQS = board.CastleRightQS(Player.White); // White Queen-side castling.
            bool castleBKS = board.CastleRightKS(Player.Black); // Black King-side castling.
            bool castleBQS = board.CastleRightQS(Player.Black); // Black Queen-side castling.

            if (!(castleWKS || castleWQS || castleBKS || castleBQS))
            {
                sb.Append('-'); // No castling rights.
                return;
            }

            if (castleWKS) sb.Append('K'); // Append 'K' for White King-side.
            if (castleWQS) sb.Append('Q'); // Append 'Q' for White Queen-side.
            if (castleBKS) sb.Append('k'); // Append 'k' for Black King-side.
            if (castleBQS) sb.Append('q'); // Append 'q' for Black Queen-side.
        }

        // Adds en passant square to the state string or '-' if no en passant is possible.
        private void AddEnPassant(Board board, Player currentPlayer)
        {
            if (!board.CanCaptureEnPassant(currentPlayer))
            {
                sb.Append('-'); // No en passant available.
                return;
            }

            Position pos = board.GetPawnSkipPosition(currentPlayer.Opponent()); // Get en passant target.
            char file = (char)('a' + pos.Column); // Convert column to file (a-h).
            int rank = 8 - pos.Row; // Convert row to rank (1-8).
            sb.Append(file);
            sb.Append(rank); // Append en passant square (e.g., 'e3').
        }
    }
}
