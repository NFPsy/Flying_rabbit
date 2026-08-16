using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI rabbitsRemainingText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private int scorePerMarble = 10;
    [SerializeField] private GameObject nextRoundButton;
    [SerializeField] private string nextSceneName;

    private int marbleCount;
    private int score;
    private int totalRabbits;
    private int rabbitsUsedUp;
    private bool gameOver;

    private void Awake()
    {
        Instance = this;
        UpdateScoreUI();
    }

    public void SetTotalRabbits(int count)
    {
        totalRabbits = count;
    }

    public void RegisterMarble()
    {
        marbleCount++;
    }

    public void OnMarbleDestroyed()
    {
        if (gameOver)
            return;

        marbleCount--;
        score += scorePerMarble;
        UpdateScoreUI();

        if (marbleCount <= 0)
            ShowResult(true);
    }

    public void UpdateRabbitsRemaining(int remaining)
    {
        if (rabbitsRemainingText != null)
            rabbitsRemainingText.text = "Rabbits: " + remaining;
    }

    public void OnRabbitDestroyed()
    {
        if (gameOver)
            return;

        rabbitsUsedUp++;
        if (rabbitsUsedUp >= totalRabbits && marbleCount > 0)
            ShowResult(false);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void ShowResult(bool cleared)
    {
        gameOver = true;

        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultText != null)
            resultText.text = cleared ? "Clear" : "Fail";

        if (nextRoundButton != null)
            nextRoundButton.SetActive(cleared && !string.IsNullOrEmpty(nextSceneName));
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextRound()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
