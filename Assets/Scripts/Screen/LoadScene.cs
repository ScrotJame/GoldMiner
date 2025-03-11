using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public Text targetScoreText;
    private int targetScore;

    void Start()
    {
        if (ScoreControl.instance != null)
        {
            targetScore = ScoreControl.instance.GetTargetScore();
            targetScoreText.text = " " + targetScore;
        }
        else
        {
            targetScoreText.text = "Target Score: Unknown";
            Debug.LogError("ScoreControl instance is null!");
        }

        StartCoroutine(LoadGamePlayAfterDelay(5f)); // Chuyển sang GamePlay sau 5 giây
    }

    IEnumerator LoadGamePlayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("GamePlay");
    }
}
