using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public GridManager gridManager;
    public Vector2Int currentBoardPosition; // Player'ýn mevcut board pozisyonu
    public float moveDelay = 1f; // Hareketler arasýndaki bekleme süresi
    private bool isMoving = false;

    void Start()
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager bulunamadý!");
            return;
        }

        // Player'ýn baþlangýç nodunu belirle
        currentBoardPosition = gridManager.WorldToBoardPosition(transform.position);
        Node startNode = gridManager.GetNode(currentBoardPosition);

        if (startNode != null)
        {
            // Player'ý baþlangýç nodunun dünya pozisyonuna taþý
            transform.position = startNode.WorldPosition;
            Debug.Log($"Player baþlangýç nodunda: {startNode.BoardPosition}");
        }
        else
        {
            Debug.LogError("Baþlangýç nodu bulunamadý!");
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector2Int direction = Vector2Int.zero;

            if (Input.GetKeyDown(KeyCode.W)) direction = -Vector2Int.up;     // Yukarý
            if (Input.GetKeyDown(KeyCode.S)) direction = -Vector2Int.down;   // Aþaðý
            if (Input.GetKeyDown(KeyCode.A))
            {
                transform.localScale = new Vector3(-1, 1, 1);
                direction = Vector2Int.left;   // Sol
            }
                
            if (Input.GetKeyDown(KeyCode.D))
            {
                transform.localScale = new Vector3(1, 1, 1);
                direction = Vector2Int.right;   // Sað
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
            // Hedef nodun dünya pozisyonuna hareket et
            Vector3 targetWorldPosition = targetNode.WorldPosition;

            while (Vector3.Distance(transform.position, targetWorldPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, 5f * Time.deltaTime);
                yield return null;
            }

            transform.position = targetWorldPosition;
            currentBoardPosition = targetPosition;
            Debug.Log($"Player yeni nodda: {currentBoardPosition}");
        }
        else
        {
            Debug.LogWarning("Hedef nod yürünebilir deðil veya geçerli deðil.");
        }

        yield return new WaitForSeconds(moveDelay);
        isMoving = false;
    }
}
