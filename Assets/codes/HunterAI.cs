using System.Collections.Generic;
using UnityEngine;

public class HunterAI
{
    private GridManager gridManager;

    public HunterAI(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    public Node GetBestMove(Node hunterNode, Node playerNode, int depth)
    {
        float alpha = float.NegativeInfinity;
        float beta = float.PositiveInfinity;

        Node bestMove = null;
        float bestValue = float.NegativeInfinity;

        foreach (Node neighbor in hunterNode.Neighbors)
        {
            if (neighbor.isWalkable)
            {
                float value = Minimax(neighbor, playerNode, depth - 1, false, alpha, beta);
                if (value > bestValue)
                {
                    bestValue = value;
                    bestMove = neighbor;
                }

                alpha = Mathf.Max(alpha, value);
                if (beta <= alpha)
                    break;
            }
        }

        return bestMove;
    }

    private float Minimax(Node hunterNode, Node playerNode, int depth, bool isMaximizing, float alpha, float beta)
    {
        if (depth == 0 || hunterNode == null || playerNode == null)
        {
            return Evaluate(hunterNode, playerNode);
        }

        if (isMaximizing)
        {
            float maxEval = float.NegativeInfinity;

            foreach (Node neighbor in hunterNode.Neighbors)
            {
                if (neighbor.isWalkable)
                {
                    float eval = Minimax(neighbor, playerNode, depth - 1, false, alpha, beta);
                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);

                    if (beta <= alpha)
                        break;
                }
            }

            return maxEval;
        }
        else
        {
            float minEval = float.PositiveInfinity;

            foreach (Node neighbor in playerNode.Neighbors)
            {
                if (neighbor.isWalkable)
                {
                    float eval = Minimax(hunterNode, neighbor, depth - 1, true, alpha, beta);
                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);

                    if (beta <= alpha)
                        break;
                }
            }

            return minEval;
        }
    }

    private float Evaluate(Node hunterNode, Node playerNode)
    {
        float distanceToPlayer = Vector3.Distance(hunterNode.WorldPosition, playerNode.WorldPosition);
        return -distanceToPlayer; // Oyuncuya yaklaþmak için negatif mesafe.
    }
}
