using UnityEngine;

public class Miner: MonoBehaviour
{
    private static Miner instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
