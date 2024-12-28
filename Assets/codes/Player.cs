using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public GridManager gridManager;
    public Vector2Int currentBoardPosition; // Player'ýn mevcut board pozisyonu
    public float moveDelay = 1f; // Hareketler arasýndaki bekleme süresi
    private bool isMoving = false;
    private Node currentNode;
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
        currentNode=startNode;
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

            if (Input.GetKey(KeyCode.W)) direction = -Vector2Int.up;     // Yukarý
            if (Input.GetKey(KeyCode.S)) direction = -Vector2Int.down;   // Aþaðý
            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector3(-1, 1, 1);
                direction = Vector2Int.left;   // Sol
            }
                
            if (Input.GetKey(KeyCode.D))
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
            CheckForGold(targetNode);
        }
        else
        {
            Debug.LogWarning("Hedef nod yürünebilir deðil veya geçerli deðil.");
        }

        yield return new WaitForSeconds(moveDelay);
        isMoving = false;
    }

    void CheckForGold(Node node)
    {
        // Collider'larý kontrol et
        Collider2D[] colliderCheck = Physics2D.OverlapCircleAll(node.WorldPosition, 0.1f); // 2D oyun için
        foreach (Collider2D collider in colliderCheck)
        {
            if (collider.CompareTag("Gold")) // "Gold" tag'ine sahip altýný bulduk
            {
                Destroy(collider.gameObject); // Altýný yok et
                node.SetGold(false); // Node üzerindeki altýn bilgisini güncelle
                Debug.Log($"Altýn toplandý: {node.BoardPosition}");
                break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Eðer çarpýþýlan obje "Merdiven" tagine sahipse
        if (collision.CompareTag("Merdiven"))
        {
            Debug.Log("Merdivene çarptýn!");

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (collision.CompareTag("Hunter"))
        {
            Debug.Log("Merdivene çarptýn!");

            
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
