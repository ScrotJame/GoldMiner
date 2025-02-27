using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameControll : MonoBehaviour
{
    public static GameControll instance;
    [SerializeField]
    private Text ScoreTarget, _Time, Number, Score, _Notif;
    private int timeLeft, targetScore;
    private Coroutine countdownCoroutine;
    [SerializeField] GameObject _MenuGamePanel, _GameWinPanel;
    public Button Playbutt, Homebutt, Againbutt;

    public bool isAlive = true;
    public int count = 0;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        InitializeLevel();
    }
    private void Update()
    {
    }

    void Start()
    {
        StartCountdown();
        if (_GameWinPanel == null) { return; }
        _GameWinPanel.SetActive(false);
        if (_MenuGamePanel == null) { return; }
        _MenuGamePanel.SetActive(false);
    }

    private void StartCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        countdownCoroutine = StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        while (timeLeft > 0)
        {
            _Time.text = timeLeft.ToString("F0");
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        _Time.text = "0";
        CheckMissionComplete(0, timeLeft);
    }

    private void InitializeLevel()
    {
        targetScore = 1000;
        timeLeft = 60;
        _SetScoreTarget(targetScore);
        _SetTime(timeLeft);
    }

    public void _SetScoreTarget(int target)
    {
        targetScore = target;
        ScoreTarget.text = target.ToString();
    }

    public void _SetTime(int time)
    {
        timeLeft = time;
        _Time.text = time.ToString();
    }

    public void _SetScore()
    {
        Score.text = "" + 0;
    }
    public void _getScore(int score)
    {
        Score.text = "" + score;
        CheckMissionComplete(score, timeLeft);
    }

    public void _SetNumber()
    {
        Number.text = "" +0;
    }
    public void _getNumber(int number)
    {
        Number.text = "" + number;
    }
    private void CheckMissionComplete(int score, int time)
    {
        if (score >= targetScore)
        {
                Debug.Log("Mission Complete!");
                _Notif.text = "Mission Complete!";
                _GameWinPanel.SetActive(true);
                StopCountdown();
            Time.timeScale = 1;
        }
        else if (time <= 0)
        {
            Debug.Log("Mission Failed!");
            _Notif.text = "Mission Failed! \n Do you want to try again?";
            _MenuGamePanel.SetActive(true);
            Againbutt.gameObject.SetActive(true);
        }
    }

    private void StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
    }
    public void _HomeButton()
    {
        Application.LoadLevel("MainMenu");
    }
    public void _PlayAgainButton()
    {
        Application.LoadLevel("GamePlay");
    }
    public void _PlayButton()
    {
        _MenuGamePanel.SetActive(false);
        _GameWinPanel.SetActive(false);
        Time.timeScale = 1;
        StartCountdown();
        AdvanceToNextLevel(int.Parse(Score.text));
    }
    public void _ContinueButton()
    {
        _MenuGamePanel.SetActive(false);
        _GameWinPanel.SetActive(false);
        Time.timeScale = 1;
        StartCountdown();
    }

    public void _StopBtutton()
    {
        _MenuGamePanel.SetActive(true);
        _GameWinPanel.SetActive(false);
        StopCountdown();
        Time.timeScale = 0; 
    }

    private void AdvanceToNextLevel(int score)
    {
        targetScore += score / 2;
        timeLeft += 30;
        _SetScoreTarget(targetScore);
        _SetTime(timeLeft);
        StartCountdown();
    }
}


