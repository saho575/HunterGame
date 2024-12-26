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
    private Coroutine moveCoroutine;

    void Start()
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager bulunamadý!");
            return;
        }

        startNode = gridManager.GetNode(gridManager.WorldToBoardPosition(transform.position));
        targetNode = gridManager.GetNode(gridManager.WorldToBoardPosition(Target.transform.position));

        CalculatePath(); // Yolu hesapla
        StartMovement();
    }

    void Update()
    {
        Node newTargetNode = gridManager.GetNode(gridManager.WorldToBoardPosition(Target.transform.position));
        startNode = gridManager.GetNode(gridManager.WorldToBoardPosition(transform.position));
        if (targetNode == null || targetNode.BoardPosition != newTargetNode.BoardPosition)
        {
            // Hedef deðiþtiðinde yolu yeniden hesapla ve hareketi sýfýrla
            targetNode = newTargetNode;
            CalculatePath();
            StartMovement();
        }

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
        else
        {
            currentPathIndex = 0; // Yeni yol için index sýfýrla
        }
    }

    void StartMovement()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        if (path != null && path.Count > 0)
        {
            moveCoroutine = StartCoroutine(MoveAlongPath());
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
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}