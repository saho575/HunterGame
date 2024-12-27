using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
public class GridManager : MonoBehaviour
{
    public int gridSizeX, gridSizeY;
    private Node[,] board;  // Resources klasöründe olmalı.
    public Tilemap tilemap;
    public GameObject goldPrefab;

    void Awake()
    {
        LoadMapFromData();
        SetNeighbors();
    }

    void LoadMapFromData()
    {
        gridSizeX = getScene().GetLength(1); // Sütun sayısı (X boyutu)
        gridSizeY = getScene().GetLength(0); // Satır sayısı (Y boyutu)

        board = new Node[gridSizeX, gridSizeY];

        // World Position başlangıcı (sol üst köşe)
        Vector3 startPosition = new Vector3(0.5f, -3.5f, 0);

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                int cellValue = getScene()[y, x];

                // Sol üst köşeye göre worldPosition hesaplanıyor.
                Vector3 worldPosition = startPosition + new Vector3(x, -y, 0);

                Node node = new Node();
                node.InitializeNode(new Vector2Int(x, y), worldPosition);

                // '1' -> Engel, '2' -> Altın, '0' -> Yürünebilir
                if (cellValue == 1)
                {
                    node.isWalkable = false;
                }
                else if (cellValue == 2)
                {
                    // Altın yerleştir
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
        Debug.Log("adna");
        if (board == null) return;
        Debug.Log("urfa");
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
            case 0:
                return GridData.Map1;
            case 1:
                return GridData.Map2;
            default:
                Debug.LogWarning("Sahnede tanımlı bir harita yok, varsayılan Map1 seçiliyor.");
                return GridData.Map1;
        }
    }
    
}
