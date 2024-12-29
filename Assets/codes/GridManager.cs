using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public int gridSizeX, gridSizeY;
    private Node[,] board;
    public Tilemap tilemap;
    public GameObject goldPrefab;

    void Awake()
    {
        LoadMapFromData();
        SetNeighbors();
    }

    void LoadMapFromData()
    {
        gridSizeX = getScene().GetLength(1); // Column count (X dimension).
        gridSizeY = getScene().GetLength(0); // Row count (Y dimension).

        board = new Node[gridSizeX, gridSizeY];

        // World Position başlangıcı (sol üst köşe)
        Vector3 startPosition = new Vector3(0.5f, -3.5f, 0);

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                int cellValue = getScene()[y, x];

                // Starting position in world coordinates (top-left corner).
                Vector3 worldPosition = startPosition + new Vector3(x, -y, 0);

                Node node = new Node();
                node.InitializeNode(new Vector2Int(x, y), worldPosition);

                // '1' -> Obstacle, '2' -> Gold, '0' -> Walkable
                if (cellValue == 1)
                {
                    node.isWalkable = false;
                }
                else if (cellValue == 2)
                {
                    // Place gold.
                    node.SetGold(true);
                    SpawnGold(worldPosition);
                }

                board[x, y] = node;
            }
        }

        Debug.Log("Map loaded from GridData with top-left as (0, 0).");
    }

    void SpawnGold(Vector3 position)
    {
        Instantiate(goldPrefab, position, Quaternion.identity);
    }

    void SetNeighbors()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node currentNode = board[x, y];

                if (x > 0) currentNode.AddNeighbor(board[x - 1, y]);
                if (x < gridSizeX - 1) currentNode.AddNeighbor(board[x + 1, y]);
                if (y > 0) currentNode.AddNeighbor(board[x, y - 1]);
                if (y < gridSizeY - 1) currentNode.AddNeighbor(board[x, y + 1]);
            }
        }

        Debug.Log("Neighbors set.");
    }

    void OnDrawGizmos()
    {
        if (board == null) return;
        foreach (var node in board)
        {
            Gizmos.color = node.isWalkable ? Color.green : Color.red;
            Gizmos.DrawSphere(node.worldPosition, 0.1f);
        }
    }

    public Vector2Int WorldToBoardPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - .5f));
        int y = Mathf.RoundToInt(-(worldPos.y + 3.5f));  //-3.5 + 3.5 =0    -4.5+3.5=-1
        return new Vector2Int(x, y);
    }

    public Node GetNode(Vector2Int position)
    {
        if (position.x >= 0 && position.x < gridSizeX && position.y >= 0 && position.y < gridSizeY)
            return board[position.x, position.y];
        return null;
    }

    public int[,] getScene()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                return GridData.Map1;
            case 2:
                return GridData.Map2;
            case 3:
                return GridData.Map3;
            default:
                Debug.LogWarning("No defined map found in the scene, defaulting to Map1.");
                return GridData.Map1;
        }
    }

    public List<Node> GetGoldNodes()
    {
        List<Node> goldNodes = new List<Node>();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (board[x, y].hasGold)
                {
                    goldNodes.Add(board[x, y]);
                }
            }
        }

        return goldNodes;
    }
}
