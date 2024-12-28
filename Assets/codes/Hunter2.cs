using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Hunter2 : MonoBehaviour
{
    public Node startNode, targetNode;
    public GridManager gridManager;
    private Node bestMove; // En iyi hareket
    public float moveSpeed = 2f; // Hareket hýzý
    public GameObject Target; // Oyuncu
    public int searchDepth = 5; // Min-max algoritmasý derinliði
    public int weight1 = 2, weight2 = 1;
    private Coroutine moveCoroutine;

    Node previousHunterNode = null; // Avcýnýn bir önceki düðümü
    float captureThreshold = 1.0f; // Yakalama mesafesi
    float penaltyForRevisiting = 10.0f; // Ayný düðüme dönme cezasý

    Astar astar = new Astar(); // A* algoritmasý örneði
    Dictionary<(Node, Node), float> distanceCache = new Dictionary<(Node, Node), float>(); // Mesafe önbelleði

    void Start()
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager bulunamadý!");
            return;
        }

        startNode = gridManager.GetNode(gridManager.WorldToBoardPosition(transform.position));
        targetNode = gridManager.GetNode(gridManager.WorldToBoardPosition(Target.transform.position));

        StartMovement();
    }

    void Update()
    {
        Node newTargetNode = gridManager.GetNode(gridManager.WorldToBoardPosition(Target.transform.position));
        startNode = gridManager.GetNode(gridManager.WorldToBoardPosition(transform.position));

        if (targetNode == null || targetNode.BoardPosition != newTargetNode.BoardPosition)
        {
            targetNode = newTargetNode;
            StartMovement();
        }

        Flip();
    }

    void StartMovement()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // Min-max algoritmasý ile en iyi hareketi hesapla
        bestMove = GetBestMove();
        if (bestMove != null)
        {
            moveCoroutine = StartCoroutine(MoveToNode(bestMove));
        }
    }

    Node GetBestMove()
    {
        List<Node> neighbors = startNode.Neighbors;
        float bestValue = float.NegativeInfinity;
        Node bestMove = null;

        foreach (Node neighbor in neighbors)
        {
            if (neighbor.isWalkable)
            {
                float value = Minimax(neighbor, targetNode, searchDepth, false, float.NegativeInfinity, float.PositiveInfinity);
                if (value > bestValue)
                {
                    bestValue = value;
                    bestMove = neighbor;
                }
            }
        }

        return bestMove;
    }

    float Minimax(Node currentNode, Node playerNode, int depth, bool isMaximizing, float alpha, float beta)
    {
        if (depth == 0 || currentNode == null || playerNode == null)
        {
            return Evaluate(currentNode, playerNode);
        }

        if (isMaximizing)
        {
            float maxEval = float.NegativeInfinity;

            foreach (Node neighbor in currentNode.Neighbors)
            {
                if (neighbor.isWalkable)
                {
                    float eval = Minimax(neighbor, playerNode, depth - 1, false, alpha, beta);
                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);

                    if (beta <= alpha)
                        break; // Alfa-beta budama
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
                    float eval = Minimax(currentNode, neighbor, depth - 1, true, alpha, beta);
                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);

                    if (beta <= alpha)
                        break; // Alfa-beta budama
                }
            }

            return minEval;
        }
    }

    float Evaluate(Node hunterNode, Node playerNode)
    {
        // Basit bir heuristik: Hunter oyuncuya ne kadar yakýnsa o kadar avantajlý
        float hunterToPlayer = Vector3.Distance(hunterNode.WorldPosition, playerNode.WorldPosition);

        // Oyuncunun bir altýna yakýn olmasý durumunu cezalandýrabiliriz
        List<Node> goldNodes = gridManager.GetGoldNodes();
        float playerToGold = float.PositiveInfinity;
        foreach (var goldNode in goldNodes)
        {
            if (goldNode.hasGold)
            {
                float distance = Vector3.Distance(playerNode.WorldPosition, goldNode.WorldPosition);
                playerToGold = Mathf.Min(playerToGold, distance);
            }
        }

        return weight1 * (-hunterToPlayer) + weight2 * (1f / (playerToGold + 1f)); // Daha iyi sonuç için aðýrlýklar eklenebilir
    }

    float GetCachedDistance(Node start, Node end)
    {
        if (distanceCache.TryGetValue((start, end), out float cachedDistance))
        {
            return cachedDistance;
        }
        else
        {
            float distance = astar.FindPath(start, end).Count;
            distanceCache[(start, end)] = distance;
            return distance;
        }
    }

    IEnumerable<Node> PredictPlayerMoves(Node playerNode)
    {
        return playerNode.Neighbors.Where(neighbor => neighbor.isWalkable);
    }

    IEnumerator MoveToNode(Node targetNode)
    {
        Vector3 targetWorldPosition = targetNode.WorldPosition;

        while (Vector3.Distance(transform.position, targetWorldPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        previousHunterNode = startNode;
        startNode = targetNode;
    }

    void Flip()
    {
        if (Target.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
