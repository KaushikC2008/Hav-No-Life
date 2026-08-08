using UnityEngine;

public class LevelFinishTrigger : MonoBehaviour
{
    [SerializeField] private GameObject levelFinishedPanel;

    private bool levelFinished = false;

    private void Start()
    {
        if (levelFinishedPanel != null)
        {
            levelFinishedPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (levelFinished)
            return;

        if (!other.CompareTag("Player"))
            return;

        CheckLevelProgress();
    }

    private void CheckLevelProgress()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.IsDemoFinished())
        {
            levelFinished = true;

            Debug.Log("Level requirements completed!");

            if (levelFinishedPanel != null)
            {
                levelFinishedPanel.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Level requirements have not been completed yet.");

            int requiredEnemies = Mathf.CeilToInt(
                GameManager.Instance.TotalNormalEnemies * 0.9f
            );

            Debug.Log(
                $"Progress: " +
                $"{GameManager.Instance.DefeatedNormalEnemies}/{requiredEnemies} " +
                $"normal enemies | " +
                $"Boss defeated: {GameManager.Instance.BossDefeated}"
            );
        }
    }
}
