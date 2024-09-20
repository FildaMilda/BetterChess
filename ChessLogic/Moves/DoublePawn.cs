namespace ChessLogic
{
    public class DoublePawn : Move
    {
        // Defines the double pawn move, inheriting from the Move class.
        public override MoveType Type => MoveType.DoublePawn;
        public override Position FromPos { get; }
        public override Position ToPos { get; }

        private readonly Position skippedPos;

        // Constructor initializes the starting and ending positions and calculates the skipped position.
        public DoublePawn(Position from, Position to)
        {
            FromPos = from;
            ToPos = to;
            skippedPos = new Position((from.Row + to.Row) / 2, from.Column); // Position the pawn skips over.
        }

        // Executes the double pawn move on the board and marks the skipped position for en passant.
        public override bool Execute(Board board)
        {
            Player player = board[FromPos].Color;
            board.SetPawnSkipPosition(player, skippedPos); // Marks the skipped position for en passant.
            new NormalMove(FromPos, ToPos).Execute(board); // Executes the actual move.

            return true;
        }
    }
}

