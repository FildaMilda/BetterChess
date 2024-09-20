using ChessLogic;

namespace MinMax
{
    internal class Tests
    {
        static void Main(string[] args)
        {
            // Define test cases using FEN notation and expected best moves
            Console.WriteLine("Running Tests...\n");
            int maxDepth = 5;
            List<(string FEN, string BestMove)> testCases = new List<(string, string)>
            {
                ("8/3p4/2pBk2K/2P1P3/p1Pp4/3P1P2/8/8 w - - 0 1", "h6g6"),
                ("k7/8/1K4P1/8/8/8/8/8 w - - 0 1", "g6g7"),
                ("k7/8/1K1P4/8/p7/8/8/8 w - - 0 1", "d6d7"),
                ("6k1/5ppp/Rp3n2/P7/8/8/5PPP/6K1 w - - 0 1", "a6a8"),
                ("k7/7Q/1K6/8/8/8/8/8 w - - 0 1", "h7h8"),
                ("k7/6K1/8/8/8/7P/8/8 w - - 0 1", "null")
            };

            // Display test header
            Console.WriteLine("Test index\tBest move\tPrediction\tIs correct");

            // Loop through test cases and evaluate using the Minimax algorithm
            for (int i = 0; i < testCases.Count; i++)
            {
                string fen = testCases[i].FEN;
                string bestMove = testCases[i].BestMove;

                GameState gameState = new GameState(fen); // Initialize game state with FEN
                Move prediction = MiniMaxAlgorithm.FindBestMove(gameState, maxDepth); // Find best move
                string predString = Board.ConvertMove(prediction); // Convert move to string
                Console.WriteLine(i + "\t\t" + bestMove + "\t\t" + predString + "\t\t" + (predString == bestMove)); // Output result
            }
        }
    }
}
