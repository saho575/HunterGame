using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Merdiven prefab�n� referans edin
    public GameObject merdivenPrefab;

    // GridManager referans�
    public GridManager gridManager;

    void Update()
    {
        // Sahnede "Alt�n" tagine sahip objeleri kontrol et
        GameObject[] altinlar = GameObject.FindGameObjectsWithTag("Gold");

        // E�er hi� "Alt�n" kalmad�ysa
        if (altinlar.Length == 0)
        {
            // E�er merdiven hen�z olu�turulmad�ysa
            if (!GameObject.FindGameObjectWithTag("Merdiven"))
            {
                // Y�r�nebilir bir node se� ve merdiven olu�tur
                CreateLadder();
            }
        }
    }

    void CreateLadder()
    {
        Node randomNode = GetRandomWalkableNode();
        if (randomNode != null)
        {
            // Merdiven olu�tur
            Instantiate(merdivenPrefab, randomNode.WorldPosition, Quaternion.identity).tag = "Merdiven";
            Debug.Log($"Merdiven olu�turuldu! Konum: {randomNode.BoardPosition}");
        }
        else
        {
            Debug.LogWarning("Y�r�nebilir bir node bulunamad�, merdiven olu�turulamad�!");
        }
    }

    Node GetRandomWalkableNode()
    {
        Node selectedNode = null;
        int gridSizeX = gridManager.gridSizeX;
        int gridSizeY = gridManager.gridSizeY;

        do
        {
            // Rastgele bir x ve y koordinat� se�
            int randomX = Random.Range(0, gridSizeX);
            int randomY = Random.Range(0, gridSizeY);

            // Se�ilen koordinatlardaki nodu al
            selectedNode = gridManager.GetNode(new Vector2Int(randomX, randomY));

            // Nodu kontrol et: ge�erli mi ve y�r�nebilir mi?
        } while (selectedNode == null || !selectedNode.isWalkable);

        return selectedNode;
    }
}
