using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject deathMenuUI;

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (deathMenuUI != null) deathMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (deathMenuUI == null || !deathMenuUI.activeSelf))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ShowDeathMenu()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (deathMenuUI != null) deathMenuUI.SetActive(true);
        
        Time.timeScale = 0f;
    }

    public void RetryCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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