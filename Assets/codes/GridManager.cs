using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    private int gridSizeX = 27, gridSizeY = 14;
    private float nodeSpacing = 1f;
    private Node[,] board;
    public Tilemap tilemap;
    private int[,] obs;

    void Awake()
    {
        InitializeBoard();
        SetNeighbors();
        AddObs(new int[,] {
            // Büyük U - Sol
            {5, 2}, {6, 2}, {7, 2}, {8, 2}, {9, 2},
            {9, 3}, {9, 4}, {9, 5},
            {8, 5}, {7, 5}, {6, 5}, {5, 5},

            // Büyük U - Sağ (simetrik)
            {5, 10}, {6, 10}, {7, 10}, {8, 10}, {9, 10},
            {9, 9}, {9, 8}, {9, 7},
            {8, 7}, {7, 7}, {6, 7}, {5, 7},

            {18, 5}, {18, 6}, {18, 7}, {18, 8}, {18, 9},
            {17, 9}, {16, 9}, {15, 9},
            {15, 8}, {15, 7}, {15, 6}, {15, 5},

            {20,10 },{20,11 },{21,11 },{22,11 },{23,11 },{24,11 }
        });


    }

    public Vector2Int WorldToBoardPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - .5f) );
        int y = Mathf.RoundToInt(-(worldPos.y + 3.5f) );  //-3.5 + 3.5 =0    -4.5+3.5=-1
        return new Vector2Int(x, y);
    }

    public Node GetNode(Vector2Int position)
    {
        if (position.x >= 0 && position.x < gridSizeX && position.y >= 0 && position.y < gridSizeY)
            return board[position.x, position.y];
        return null;
    }

    void InitializeBoard()
    {
        board = new Node[gridSizeX, gridSizeY];

        Vector3 startPos = new Vector3(0.5f, -3.5f, 0);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPosition = startPos + new Vector3(x * nodeSpacing, -y * nodeSpacing, 0);


                Node node = new Node();
                Vector2Int boardPosition = new Vector2Int(y, x);

                node.InitializeNode(boardPosition, worldPosition);
                board[x, y] = node;
            }
        }

        Debug.Log("Tahta oluşturuldu.");
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

        Debug.Log("Komşular belirlendi.");
    }

    public void SetNodeUnwalkable(int x, int y)
    {
        if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
        {
            board[x, y].isWalkable = false;
            //Debug.Log($"Node at ({x}, {y}) is now unwalkable.");
        }
        else
        {
            Debug.LogWarning($"Invalid node position ({x}, {y}).");
        }
    }

    public void AddObs(int[,] obstacles)
    {
        obs = obstacles;
        for (int i = 0; i < obs.GetLength(0); i++)
        {
            int x = obs[i, 0];
            int y = obs[i, 1];
            SetNodeUnwalkable(x, y);
        }
        Debug.Log("Obstacles added and set as unwalkable.");
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
}
