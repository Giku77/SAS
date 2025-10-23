using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject overUI;
    public GameObject endUI;

    public void ShowOverUI()
    {
        overUI.SetActive(true);
    }

    public void ShowEndUI()
    {
        Time.timeScale = 0f;
        endUI.SetActive(true);
    }

    public void ExitGame()
    {
       Application.Quit();
    }
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
