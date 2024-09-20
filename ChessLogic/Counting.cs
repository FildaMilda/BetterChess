namespace ChessLogic
{
    public class Counting
    {
        // Dictionaries to keep track of the piece count for white and black pieces
        private readonly Dictionary<PieceType, int> whiteCount = new();
        private readonly Dictionary<PieceType, int> blackCount = new();

        // Stores the total count of all pieces on the board
        public int TotalCount { get; private set; }

        // Constructor initializes both white and black counts to zero for each piece type
        public Counting()
        {
            foreach (PieceType type in Enum.GetValues(typeof(PieceType)))
            {
                whiteCount[type] = 0;
                blackCount[type] = 0;
            }
        }

        // Increments the count for a specific piece type based on the player's color
        public void Increment(Player color, PieceType type)
        {
            if (color == Player.White)
            {
                whiteCount[type]++;
            }
            else if (color == Player.Black)
            {
                blackCount[type]++;
            }

            // Increases the total piece count
            TotalCount++;
        }

        // Returns the count of white pieces of a specific type
        public int White(PieceType type)
        {
            return whiteCount[type];
        }

        // Returns the count of black pieces of a specific type
        public int Black(PieceType type)
        {
            return blackCount[type];
        }
    }
}
