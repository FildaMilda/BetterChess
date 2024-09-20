using ChessLogic;

namespace MinMax
{
    public class MiniMaxAlgorithm
    {
        // Method to find the best move using Minimax with a given depth
        public static Move FindBestMove(GameState gameState, int depth)
        {
            Move bestMove = null;
            int bestScore = int.MinValue;

            // Iterate through all legal moves for the current player
            foreach (Move move in gameState.AllLegalMovesFor(gameState.CurrentPlayer))
            {
                // Create a copy of the game state and make the move
                GameState newState = new GameState(gameState.CurrentPlayer, gameState.Board.Copy());
                newState.MakeMove(move);

                // Use Minimax algorithm to evaluate the move's score
                int score = MiniMax(newState, depth - 1, false);
                if (score > bestScore)
                {
                    bestScore = score; // Update best score and move if score is better
                    bestMove = move;
                }
            }

            // Return best move found or null if no valid moves (i.e., score == 0)
            return (bestScore == 0) ? null : bestMove;
        }

        // Recursive Minimax method to evaluate game states
        static int MiniMax(GameState gameState, int depth, bool isMaximizingPlayer)
        {
            // Base case: if game over or depth limit reached, evaluate the board
            if (gameState.IsGameOver() || depth == 0)
            {
                return EvaluateGameState(gameState);
            }

            // Maximizing player's turn
            if (isMaximizingPlayer)
            {
                int maxEval = int.MinValue;

                // Check all legal moves for maximizing player
                foreach (Move move in gameState.AllLegalMovesFor(gameState.CurrentPlayer))
                {
                    GameState newState = new GameState(gameState.CurrentPlayer, gameState.Board.Copy());
                    newState.MakeMove(move);
                    int eval = MiniMax(newState, depth - 1, false); // Recurse with minimizing player
                    maxEval = Math.Max(maxEval, eval); // Get maximum score
                }

                return maxEval;
            }
            // Minimizing player's turn
            else
            {
                int minEval = int.MaxValue;

                // Check all legal moves for minimizing player
                foreach (Move move in gameState.AllLegalMovesFor(gameState.CurrentPlayer))
                {
                    GameState newState = new GameState(gameState.CurrentPlayer, gameState.Board.Copy());
                    newState.MakeMove(move);
                    int eval = MiniMax(newState, depth - 1, true); // Recurse with maximizing player
                    minEval = Math.Min(minEval, eval); // Get minimum score
                }

                return minEval;
            }
        }

        // Evaluate the game state at the end of the recursion
        static int EvaluateGameState(GameState gameState)
        {
            // Return 0 if game is not over
            if (!gameState.IsGameOver()) return 0;

            // Return evaluation based on game result
            Player winner = gameState.Result.Winner;

            return winner switch
            {
                Player.White => 1, // Maximizing player wins
                Player.Black => -1, // Minimizing player wins
                _ => 0 // Draw
            };
        }
    }
}
