namespace ChessLogic
{
    public class EnPassant : Move
    {
        // Defines the en passant move, inheriting from the Move class.
        public override MoveType Type => MoveType.EnPassant;
        public override Position FromPos { get; }
        public override Position ToPos { get; }

        private readonly Position capturePos;

        // Constructor initializes the starting, ending, and capture positions for en passant.
        public EnPassant(Position from, Position to)
        {
            FromPos = from;
            ToPos = to;
            capturePos = new Position(from.Row, to.Column); // The position of the pawn being captured.
        }

        // Executes the en passant move, moving the pawn and capturing the opponent's pawn.
        public override bool Execute(Board board)
        {
            new NormalMove(FromPos, ToPos).Execute(board); // Moves the pawn.
            board[capturePos] = null; // Removes the captured pawn from the board.

            return true;
        }
    }
}
