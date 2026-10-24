
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameplaySceneName = "SampleScene";

    public void PlayGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }
}
