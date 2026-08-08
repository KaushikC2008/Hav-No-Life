using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuUI;

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        // Only trigger pause menu with Escape during normal gameplay
        if (Input.GetKeyDown(KeyCode.Escape))
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

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        SaveCurrentPlayerProgress();
        
        if (SceneTransition.instance != null)
        {
            SceneTransition.instance.LoadScene("MainMenuScene");
        }
        else
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    private void SaveCurrentPlayerProgress()
    {
        if (GameManager.Instance != null && GameManager.Instance.PlayerData != null)
        {
            PlayerCharacter player = FindAnyObjectByType<PlayerCharacter>();
            if (player != null)
            {
                GameManager.Instance.PlayerData.currentHealth = player.GetCurrentHealth();
                GameManager.Instance.PlayerData.currentManaPoints = player.GetCurrentMana();
                GameManager.Instance.PlayerData.latestCheckpointPosition = player.transform.position;
                GameManager.Instance.PlayerData.hasCheckpoint = true;
                
                Debug.Log("[GameMenuController] Player progress saved before quitting to main menu.");
            }
        }
    }
}