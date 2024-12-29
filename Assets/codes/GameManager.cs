using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject ladderPrefab;
    public static int tryAgain = 3;
    public GridManager gridManager;

    void Update()
    {
        GameObject[] goldObjects = GameObject.FindGameObjectsWithTag("Gold");

        if (goldObjects.Length == 0)
        {
            if (!GameObject.FindGameObjectWithTag("Ladder"))
            {
                CreateLadder();
            }
        }
    }

    void CreateLadder()
    {
        Node randomNode = GetRandomWalkableNode();
        if (randomNode != null)
        {
            Instantiate(ladderPrefab, randomNode.WorldPosition, Quaternion.identity).tag = "Ladder";
            Debug.Log($"Ladder created at position: {randomNode.BoardPosition}");
        }
        else
        {
            Debug.LogWarning("No walkable node found, unable to create ladder!");
        }
    }

    Node GetRandomWalkableNode()
    {
        Node selectedNode = null;
        int gridSizeX = gridManager.gridSizeX;
        int gridSizeY = gridManager.gridSizeY;

        do
        {
            int randomX = Random.Range(0, gridSizeX);
            int randomY = Random.Range(0, gridSizeY);

            selectedNode = gridManager.GetNode(new Vector2Int(randomX, randomY));

        } while (selectedNode == null || !selectedNode.isWalkable);

        return selectedNode;
    }
}
