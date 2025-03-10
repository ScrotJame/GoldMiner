using UnityEngine;
using UnityEngine.SceneManagement;

public class InputButton : MonoBehaviour
{
    
    public void _StartGameButton()
    {
        SceneManager.LoadScene("GamePlay");
    }
}
