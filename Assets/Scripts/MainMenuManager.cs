using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button continueButton;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.PlayerData != null)
        {
            continueButton.interactable = GameManager.Instance.PlayerData.hasCheckpoint;
        }
        else
        {
            continueButton.interactable = false;
        }
    }

    public void StartNewGame()
    {
        Debug.Log("Starting a fresh New Game!");

        if (GameManager.Instance != null && GameManager.Instance.PlayerData != null)
        {
            GameManager.Instance.PlayerData.currentHealth = GameManager.Instance.PlayerData.maxHealth;
            GameManager.Instance.PlayerData.currentManaPoints = GameManager.Instance.PlayerData.maxManaPoints;
            GameManager.Instance.PlayerData.hasCheckpoint = false;

            GameManager.Instance.ShouldRestorePosition = false;
        }

        LoadGameScene();
    }

    public void ContinueGame()
    {
        Debug.Log("Continuing from latest checkpoint!");

        if (GameManager.Instance != null && GameManager.Instance.PlayerData != null)
        {
            GameManager.Instance.LastPlayerPosition = GameManager.Instance.PlayerData.latestCheckpointPosition;
            GameManager.Instance.ShouldRestorePosition = true;
        }

        LoadGameScene();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game!");
        Application.Quit();
    }

    private void LoadGameScene()
    {
        if (SceneTransition.instance != null)
        {
            SceneTransition.instance.LoadScene("Tutorial Scene");
        }
        else
        {
            SceneManager.LoadScene("Tutorial Scene");
        }
    }
}
