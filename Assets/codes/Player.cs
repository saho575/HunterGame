using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public GridManager gridManager;
    public Vector2Int currentBoardPosition;
    public float moveDelay = 1f;
    private bool isMoving = false;
    private Node currentNode;

    void Start()
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }

        // Determine the player's starting node
        currentBoardPosition = gridManager.WorldToBoardPosition(transform.position);
        Node startNode = gridManager.GetNode(currentBoardPosition);
        currentNode = startNode;

        if (startNode != null)
        {
            // Transform the player to the starting node's world position
            transform.position = startNode.WorldPosition;
            Debug.Log($"Player starting at node: {startNode.BoardPosition}");
        }
        else
        {
            Debug.LogError("Starting node not found!");
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector2Int direction = Vector2Int.zero;

            if (Input.GetKey(KeyCode.W)) direction = -Vector2Int.up;
            if (Input.GetKey(KeyCode.S)) direction = -Vector2Int.down;
            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector3(-1, 1, 1);
                direction = Vector2Int.left;
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.localScale = new Vector3(1, 1, 1);
                direction = Vector2Int.right;
            }

            if (direction != Vector2Int.zero)
            {
                StartCoroutine(Move(direction));
            }
        }
    }

    IEnumerator Move(Vector2Int direction)
    {
        isMoving = true;

        Vector2Int targetPosition = currentBoardPosition + direction;
        Node targetNode = gridManager.GetNode(targetPosition);

        if (targetNode != null && targetNode.isWalkable)
        {
            // Transform to the target node's world position
            Vector3 targetWorldPosition = targetNode.WorldPosition;

            while (Vector3.Distance(transform.position, targetWorldPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, 5f * Time.deltaTime);
                yield return null;
            }

            transform.position = targetWorldPosition;
            currentBoardPosition = targetPosition;
            Debug.Log($"Player moved to new node: {currentBoardPosition}");
            CheckForGold(targetNode);
        }
        else
        {
            Debug.LogWarning("Target node is not walkable or invalid.");
        }

        yield return new WaitForSeconds(moveDelay);
        isMoving = false;
    }

    void CheckForGold(Node node)
    {
        // Check for colliders
        Collider2D[] colliderCheck = Physics2D.OverlapCircleAll(node.WorldPosition, 0.1f); // For 2D games
        foreach (Collider2D collider in colliderCheck)
        {
            if (collider.CompareTag("Gold"))
            {
                Destroy(collider.gameObject); // Destroy the gold
                node.SetGold(false); // Update node's gold status
                Debug.Log($"Gold collected at: {node.BoardPosition}");
                break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // If the collided object has the "Ladder" tag
        if (collision.CompareTag("Ladder"))
        {
            Debug.Log("Collided with a ladder!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (collision.CompareTag("Hunter"))
        {
            Debug.Log("Collided with a hunter!");

            GameManager.tryAgain--;
            Debug.Log(GameManager.tryAgain);

            if (GameManager.tryAgain == 0)
            {
                SceneManager.LoadScene(4);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
