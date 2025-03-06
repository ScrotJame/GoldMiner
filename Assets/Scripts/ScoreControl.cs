using UnityEngine;
using UnityEngine.UI;

public class ScoreControl : MonoBehaviour
{
    public static ScoreControl instance;
    public Text Score,  ScoreTarget; 
    private int currentScore, targetScore;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("ScoreControl instance initialized.");
        }
        else if (instance != this)
        {
            Debug.Log("Duplicate ScoreControl found, destroying: " + gameObject.name);
            Destroy(gameObject);
            return;
        }
        if (Score == null)
        {
            GameObject scoreObj = GameObject.Find("ScoreText");
            if (scoreObj != null)
            {
                Score = scoreObj.GetComponent<Text>();
                Debug.Log("ScoreText assigned.");
            }
            else
            {
                Debug.LogError("ScoreText not found in scene!");
            }
        }

        if (ScoreTarget == null)
        {
            GameObject scoreTargetObj = GameObject.Find("ScoreTargetText");
            if (scoreTargetObj != null)
            {
                ScoreTarget = scoreTargetObj.GetComponent<Text>();
                Debug.Log("ScoreTargetText assigned.");
            }
            else
            {
                Debug.LogError("ScoreTargetText not found in scene!");
            }
        }
    }

    public void InitializeScore(int initialScore = 0, int target = 1000)
    {
        currentScore = initialScore;
        targetScore = target;
        UpdateScoreUI();
        UpdateTargetScoreUI();
    }

    public void AddScore(int amount)
    {
        if (Score == null) return;
        currentScore += amount;
        UpdateScoreUI();
        SavePlayerData();
    }

    public bool SpendScore(int amount)
    {
        if (Score == null) return false;
        if (currentScore >= amount)
        {
            currentScore -= amount;
            UpdateScoreUI();
            SavePlayerData();
            return true;
        }
        return false;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetTargetScore()
    {
        return targetScore;
    }

    public void SetTargetScore(int target)
    {
        targetScore = target;
        UpdateTargetScoreUI();
    }
    public void LoadPlayerData()
    {
        currentScore = PlayerPrefs.GetInt("PlayerScore", 0);
        UpdateScoreUI();
    }

    private void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerScore", currentScore);
        PlayerPrefs.Save();
    }

    private void UpdateScoreUI()
    {
        if (Score != null)
        {
            Score.text = currentScore.ToString();
        }
    }

    private void UpdateTargetScoreUI()
    {
        if (ScoreTarget != null)
        {
            ScoreTarget.text = targetScore.ToString();
        }
    }
}