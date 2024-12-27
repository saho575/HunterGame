using UnityEngine;
using UnityEngine.SceneManagement;
public class Button : MonoBehaviour
{
    public void play()
    {
        SceneManager.LoadScene(0);
    }
}
