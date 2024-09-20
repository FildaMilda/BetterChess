namespace ChessLogic
{
    // Enum to represent the player: None, White, or Black
    public enum Player
    {
        None,
        White,
        Black
    }

    // Extension methods for the Player enum
    public static class PlayerExtensions
    {
        // Method to return the opponent of the current player
        public static Player Opponent(this Player player)
        {
            return player switch
            {
                Player.White => Player.Black,  // If player is White, return Black
                Player.Black => Player.White,  // If player is Black, return White
                _ => Player.None,              // If player is None, return None
            };
        }
    }
}
