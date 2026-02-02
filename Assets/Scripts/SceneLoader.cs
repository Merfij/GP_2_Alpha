using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene("Level 1",LoadSceneMode.Additive);
        SceneManager.LoadScene("Level 2",LoadSceneMode.Additive);
        SceneManager.LoadScene("Level 3",LoadSceneMode.Additive);
        SceneManager.LoadScene("Level 4",LoadSceneMode.Additive);
        SceneManager.LoadScene("Level 5",LoadSceneMode.Additive);
        SceneManager.LoadScene("Level 6", LoadSceneMode.Additive);
        SceneManager.LoadScene("Art Corner Scene", LoadSceneMode.Additive);
    }
}
