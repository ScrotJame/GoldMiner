using UnityEngine;

public class BlindBox : MonoBehaviour
{
    public static BlindBox instance;
    private string rewardType;
    

    private void Start()
    {
        string[] rewards = { "Dynamite", "Time" };
        rewardType = rewards[Random.Range(0, rewards.Length)];
    }
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
    public string GetRewardType()
    {
        return rewardType;
    }
    
}
