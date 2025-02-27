using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SocialPlatforms.Impl;
using System;

public class GameManager : MonoBehaviour
{
    private const String Score = "Score: ", Target = "Target: ";
    public static  GameManager instance;
    public void Awake()
    {
        _isFistPlay();
    }
    public void _Save(int score)
    {
        if (!instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void _isFistPlay()
    {
        if (!PlayerPrefs.HasKey("_isFirstPlay"))
        {
            PlayerPrefs.SetInt(Score, 0);
            PlayerPrefs.SetInt("_isFirstPlay", 0);
        }
    }
    public void _setScore(int score)
    {
        PlayerPrefs.SetInt(Score, score);
    }
    public int _getScore()
    {
        return PlayerPrefs.GetInt(Score);
    }
    public int _setTargetScore()
    {
        return PlayerPrefs.GetInt(Score);
    }
    public void _setTargetScore(int target)
    {
        PlayerPrefs.SetInt(Target, target);
    }
}
