using UnityEngine;
using UnityEngine.SceneManagement;

public class InputButton : MonoBehaviour
{
    
    public void _StartGameButton()
    {
        SceneManager.LoadScene("GamePlay");
    }
    public void _ExitGameButton()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Thoát trong Editor
#endif
    }
}
