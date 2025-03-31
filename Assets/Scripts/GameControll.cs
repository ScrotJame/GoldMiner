using UnityEngine;

public class GameControll : MonoBehaviour
{
    public static GameControll instance;
    public Spawner spawner;
    public Pod pod;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        spawner = FindObjectOfType<Spawner>();
        pod = FindObjectOfType<Pod>();
    }

    public void RestartLevel()
    {
        spawner?.ResetLevel();
    }
}
