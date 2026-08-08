using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoFinishedUI : MonoBehaviour
{
    [SerializeField] private GameObject finishedPanel;

    private void Start()
    {
        finishedPanel.SetActive(false);
    }

    public void NextLevel()
    {
        SceneTransition.instance.LoadScene("Level 2");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        if (SceneTransition.instance != null)
        {
            SceneTransition.instance.LoadScene("MainMenuScene");
        }
        else
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}
