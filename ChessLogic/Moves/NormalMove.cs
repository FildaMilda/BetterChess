namespace ChessLogic
{
    // Represents a normal (standard) chess move, inheriting from the Move class.
    public class NormalMove : Move
    {
        // Specifies that this move is of type "Normal".
        public override MoveType Type => MoveType.Normal;
        // The starting position of the move.
        public override Position FromPos { get; }
        // The ending position of the move.
        public override Position ToPos { get; }

        // Constructor initializes the start and end positions of the normal move.
        public NormalMove(Position from, Position to)
        {
            FromPos = from;
            ToPos = to;
        }

        // Executes the normal move on the board, handling the movement of the piece and capturing if applicable.
        public override bool Execute(Board board)
        {
            Piece piece = board[FromPos]; // Retrieves the piece at the starting position.
            bool capture = !board.IsEmpty(ToPos); // Checks if a piece is being captured.
            board[ToPos] = piece; // Moves the piece to the target position.
            board[FromPos] = null; // Empties the starting position.
            piece.HasMoved = true; // Marks the piece as having moved.

            // Returns true if the move involves a capture or if the piece is a pawn (for special rules like en passant).
            return capture || piece.Type == PieceType.Pawn;
        }
    }
}
