using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public void _PlayButton()
    {
        SceneManager.LoadScene("GamePlay"); 
    }

}
