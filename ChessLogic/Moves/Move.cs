namespace ChessLogic
{
    // Abstract base class for different types of chess moves.
    public abstract class Move
    {
        // Defines the type of move (e.g., normal move, castling, en passant, etc.).
        public abstract MoveType Type { get; }
        // The starting position of the move.
        public abstract Position FromPos { get; }
        // The ending position of the move.
        public abstract Position ToPos { get; }

        // Executes the move on the given board. This method must be implemented by derived classes.
        public abstract bool Execute(Board board);

        // Checks if the move is legal by simulating it on a copy of the board and ensuring the player's king is not in check.
        public virtual bool IsLegal(Board board)
        {
            Player player = board[FromPos].Color;
            Board boardCopy = board.Copy();
            Execute(boardCopy); // Simulates the move.
            return !boardCopy.IsInCheck(player); // Checks if the move results in the player being in check.
        }
    }
}
