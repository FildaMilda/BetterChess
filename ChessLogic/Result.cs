namespace ChessLogic
{
    public class Result
    {
        // Property to store the winner of the game (Player.White, Player.Black, or Player.None)
        public Player Winner { get; }

        // Property to store the reason the game ended (e.g., Checkmate, Stalemate, etc.)
        public EndReason Reason { get; }

        // Constructor to initialize a result with a winner and an end reason
        public Result(Player winner, EndReason reason)
        {
            Winner = winner;
            Reason = reason;
        }

        // Static method to create a Result representing a win with Checkmate as the reason
        public static Result Win(Player winner)
        {
            return new Result(winner, EndReason.Checkmate);
        }

        // Static method to create a Result representing a draw with a specified reason (e.g., Stalemate, Draw by Agreement)
        public static Result Draw(EndReason reason)
        {
            return new Result(Player.None, reason);  // No winner in case of a draw
        }
    }
}
