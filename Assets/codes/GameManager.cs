using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Merdiven prefabýný referans edin
    public GameObject merdivenPrefab;

    // GridManager referansý
    public GridManager gridManager;

    void Update()
    {
        // Sahnede "Altýn" tagine sahip objeleri kontrol et
        GameObject[] altinlar = GameObject.FindGameObjectsWithTag("Gold");

        // Eðer hiç "Altýn" kalmadýysa
        if (altinlar.Length == 0)
        {
            // Eðer merdiven henüz oluþturulmadýysa
            if (!GameObject.FindGameObjectWithTag("Merdiven"))
            {
                // Yürünebilir bir node seç ve merdiven oluþtur
                CreateLadder();
            }
        }
    }

    void CreateLadder()
    {
        Node randomNode = GetRandomWalkableNode();
        if (randomNode != null)
        {
            // Merdiven oluþtur
            Instantiate(merdivenPrefab, randomNode.WorldPosition, Quaternion.identity).tag = "Merdiven";
            Debug.Log($"Merdiven oluþturuldu! Konum: {randomNode.BoardPosition}");
        }
        else
        {
            Debug.LogWarning("Yürünebilir bir node bulunamadý, merdiven oluþturulamadý!");
        }
    }

    Node GetRandomWalkableNode()
    {
        Node selectedNode = null;
        int gridSizeX = gridManager.gridSizeX;
        int gridSizeY = gridManager.gridSizeY;

        do
        {
            // Rastgele bir x ve y koordinatý seç
            int randomX = Random.Range(0, gridSizeX);
            int randomY = Random.Range(0, gridSizeY);

            // Seçilen koordinatlardaki nodu al
            selectedNode = gridManager.GetNode(new Vector2Int(randomX, randomY));

            // Nodu kontrol et: geçerli mi ve yürünebilir mi?
        } while (selectedNode == null || !selectedNode.isWalkable);

        return selectedNode;
    }
}
