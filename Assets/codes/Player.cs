using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public GridManager gridManager;
    public Vector2Int currentBoardPosition; // Player'�n mevcut board pozisyonu
    public float moveDelay = 1f; // Hareketler aras�ndaki bekleme s�resi
    private bool isMoving = false;
    private Node currentNode;
    void Start()
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager bulunamad�!");
            return;
        }

        // Player'�n ba�lang�� nodunu belirle
        currentBoardPosition = gridManager.WorldToBoardPosition(transform.position);
        Node startNode = gridManager.GetNode(currentBoardPosition);
        currentNode=startNode;
        if (startNode != null)
        {
            // Player'� ba�lang�� nodunun d�nya pozisyonuna ta��
            transform.position = startNode.WorldPosition;
            Debug.Log($"Player ba�lang�� nodunda: {startNode.BoardPosition}");
        }
        else
        {
            Debug.LogError("Ba�lang�� nodu bulunamad�!");
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector2Int direction = Vector2Int.zero;

            if (Input.GetKey(KeyCode.W)) direction = -Vector2Int.up;     // Yukar�
            if (Input.GetKey(KeyCode.S)) direction = -Vector2Int.down;   // A�a��
            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector3(-1, 1, 1);
                direction = Vector2Int.left;   // Sol
            }
                
            if (Input.GetKey(KeyCode.D))
            {
                transform.localScale = new Vector3(1, 1, 1);
                direction = Vector2Int.right;   // Sa�
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
            // Hedef nodun d�nya pozisyonuna hareket et
            Vector3 targetWorldPosition = targetNode.WorldPosition;

            while (Vector3.Distance(transform.position, targetWorldPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, 5f * Time.deltaTime);
                yield return null;
            }

            transform.position = targetWorldPosition;
            currentBoardPosition = targetPosition;
            Debug.Log($"Player yeni nodda: {currentBoardPosition}");
            CheckForGold(targetNode);
        }
        else
        {
            Debug.LogWarning("Hedef nod y�r�nebilir de�il veya ge�erli de�il.");
        }

        yield return new WaitForSeconds(moveDelay);
        isMoving = false;
    }

    void CheckForGold(Node node)
    {
        // Collider'lar� kontrol et
        Collider2D[] colliderCheck = Physics2D.OverlapCircleAll(node.WorldPosition, 0.1f); // 2D oyun i�in
        foreach (Collider2D collider in colliderCheck)
        {
            if (collider.CompareTag("Gold")) // "Gold" tag'ine sahip alt�n� bulduk
            {
                Destroy(collider.gameObject); // Alt�n� yok et
                node.SetGold(false); // Node �zerindeki alt�n bilgisini g�ncelle
                Debug.Log($"Alt�n topland�: {node.BoardPosition}");
                break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // E�er �arp���lan obje "Merdiven" tagine sahipse
        if (collision.CompareTag("Merdiven"))
        {
            Debug.Log("Merdivene �arpt�n!");

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (collision.CompareTag("Hunter"))
        {
            Debug.Log("Merdivene �arpt�n!");

            
            GameManager.tryagain--;
            Debug.Log(GameManager.tryagain);

            if (GameManager.tryagain == 0)
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
