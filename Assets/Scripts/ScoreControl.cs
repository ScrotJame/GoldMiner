using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreControl : MonoBehaviour
{
    public static ScoreControl instance;

    [SerializeField] private Text Score;
    [SerializeField] private Text ScoreTarget;

    private int currentScore;
    private int targetScore;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayerData(); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => AssignUIReferences();

    private void AssignUIReferences()
    {
        Score = GameObject.Find("Score")?.GetComponent<Text>();
        ScoreTarget = GameObject.Find("target")?.GetComponent<Text>();

        if (Score == null) Debug.LogError("Score UI not found in scene!");
        if (ScoreTarget == null) Debug.LogError("ScoreTarget UI not found in scene!");

        UpdateScoreUI();
        UpdateTargetScoreUI();
    }

    public void InitializeScore(int initialScore = 0, int target = 1000)
    {
        currentScore = initialScore;
        targetScore = target;
        AssignUIReferences(); // Đảm bảo UI luôn được cập nhật khi khởi tạo
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        SavePlayerData();
        UpdateScoreUI();
    }

    public bool SpendScore(int amount)
    {
        if (currentScore >= amount)
        {
            currentScore -= amount;
            SavePlayerData();
            UpdateScoreUI();
            return true;
        }
        return false;
    }

    public int GetCurrentScore() => currentScore;
    public int GetTargetScore() => targetScore;
    public void SetTargetScore(int target)
    {
        targetScore = target;
        UpdateTargetScoreUI();
    }

    private void LoadPlayerData()
    {
        currentScore = PlayerPrefs.GetInt("PlayerScore", 0);
    }

    private void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerScore", currentScore);
        PlayerPrefs.Save();
    }

    private void UpdateScoreUI()
    {
        if (Score != null) Score.text = currentScore.ToString();
    }

    private void UpdateTargetScoreUI()
    {
        if (ScoreTarget != null) ScoreTarget.text = targetScore.ToString();
    }
}
