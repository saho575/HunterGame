
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    public Node startNode, targetNode;
    public GridManager gridManager;
    private List<Node> path; // Hesaplanan yol
    private int currentPathIndex = 0;
    public float moveSpeed = 2f; // NPC hareket hýzý
    public GameObject Target;

    void Start()
    {
        
        startNode = gridManager.GetNode(gridManager.WorldToBoardPosition(transform.position));
        Debug.Log(startNode);
    
        targetNode = gridManager.GetNode(gridManager.WorldToBoardPosition(Target.transform.position));

        if (gridManager == null)
        {
            Debug.LogError("GridManager bulunamadý!");
            return;
        }

        CalculatePath(); // Yolu hesapla
        if (path != null && path.Count > 0)
        {
            StartCoroutine(MoveAlongPath());
        }
    }

    void Update()
    {

        Flip();
    }

    void CalculatePath()
    {
        Astar astar = new Astar();
        path = astar.FindPath(startNode, targetNode);

        if (path == null)
        {
            Debug.LogWarning("Hedefe giden bir yol bulunamadý.");
        }
    }

    IEnumerator MoveAlongPath()
    {
        while (currentPathIndex < path.Count)
        {
            Node currentNode = path[currentPathIndex];
            Vector3 targetWorldPosition = currentNode.WorldPosition;

            while (Vector3.Distance(transform.position, targetWorldPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentPathIndex++;
        }

        Debug.Log("Hunter hedefe ulaþtý!");
    }

    void Flip()
    {
        if (Target.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
