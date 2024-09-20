namespace ChessLogic
{
    public class Position
    {
        // Row and column properties represent the position on the board
        public int Row { get; }
        public int Column { get; }

        // Constructor to set the row and column of the position
        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        // Determines the color of the square at the current position
        public Player SquareColor()
        {
            // If the sum of row and column is even, the square is white; otherwise, it's black
            if ((Row + Column) % 2 == 0)
            {
                return Player.White;
            }

            return Player.Black;
        }

        // Overrides the Equals method to compare two Position objects by row and column
        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Column == position.Column;
        }

        // Overrides GetHashCode to return a hash code based on row and column
        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }

        // Equality operator to compare two Position objects
        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        // Inequality operator to compare two Position objects
        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        // Operator overload to add a direction to a position, resulting in a new position
        public static Position operator +(Position pos, Direction dir)
        {
            return new Position(pos.Row + dir.RowDelta, pos.Column + dir.ColumnDelta);
        }
    }
}
