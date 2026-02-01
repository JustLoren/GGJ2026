using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string SceneName = "SampleScene";
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
