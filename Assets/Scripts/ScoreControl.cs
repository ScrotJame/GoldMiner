using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreControl : MonoBehaviour
{
    public static ScoreControl instance;
    private int currentScore = 0;
    private int targetScore, initialTime;
    private int highScore = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => UpdateUI();

    public void AddScore(int amount)
    {
        currentScore += amount;
        SaveData();
        UpdateUI();
    }

    public bool SpendScore(int amount)
    {
        if (currentScore >= amount)
        {
            currentScore -= amount;
            SaveData();
            UpdateUI();
            return true;
        }
        return false;
    }

    public int GetCurrentScore() => currentScore;
    public int GetTargetScore() => targetScore;
    public int GetHighScore() => highScore;

    public void SetTargetScore(int newTarget)
    {
        targetScore = newTarget;
        SaveData();
        UpdateUI();
    }

    public void CheckAndSaveHighScore()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveData();
        }
    }

    public void StartNewGame()
    {
        CheckAndSaveHighScore();
        ResetScore();
        SaveData();
        UpdateUI();
    }

    public void ResetScore()
    {
        currentScore = 0;
        targetScore = 650;
        initialTime =20;
        SaveData();
        UpdateUI();
    }

    public void InitializeScore(int newScore, int newTarget)
    {
        currentScore = newScore;
        targetScore = newTarget;
        SaveData();
        UpdateUI();
    }

    private void LoadData()
    {
        currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
        targetScore = PlayerPrefs.GetInt("TargetScore", 1000);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("CurrentScore", currentScore);
        PlayerPrefs.SetInt("TargetScore", targetScore);
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }

    private void UpdateUI()
    {
        UIManager.instance?.UpdateScoreUI(currentScore);
        UIManager.instance?.UpdateTargetScoreUI(targetScore);
        UIManager.instance?.UpdateHighScoreUI(highScore);
    }
}