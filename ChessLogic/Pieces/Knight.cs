namespace ChessLogic
{
    public class Knight : Piece
    {
        public override PieceType Type => PieceType.Knight;
        public override Player Color { get; }

        public Knight(Player color)
        {
            // Constructor initializes the Knight with the specified color.
            Color = color;
        }

        public override Piece Copy()
        {
            // Creates and returns a copy of the Knight, preserving its color and movement status.
            Knight copy = new Knight(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private static IEnumerable<Position> PotentialToPositions(Position from)
        {
            // Generates all potential positions the Knight can move to, based on its unique L-shaped movement pattern.
            foreach (Direction vDir in new Direction[] { Direction.North, Direction.South })
            {
                foreach (Direction hDir in new Direction[] { Direction.West, Direction.East })
                {
                    yield return from + 2 * vDir + hDir;
                    yield return from + 2 * hDir + vDir;
                }
            }
        }

        private IEnumerable<Position> MovePositions(Position from, Board board)
        {
            // Filters the potential move positions to include only those within the board boundaries
            // and positions that are either empty or occupied by an opponent's piece.
            return PotentialToPositions(from).Where(pos => Board.IsInside(pos)
                && (board.IsEmpty(pos) || board[pos].Color != Color));
        }

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            // Returns all valid moves for the Knight, converting valid positions into NormalMove objects.
            return MovePositions(from, board).Select(to => new NormalMove(from, to));
        }

    }
}