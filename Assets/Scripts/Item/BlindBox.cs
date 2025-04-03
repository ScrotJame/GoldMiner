using UnityEngine;

public class BlindBox : MonoBehaviour
{
    private string rewardType;

    private void Start()
    {
        string[] rewards = { "Dynamite", "Time" };
        rewardType = rewards[Random.Range(0, rewards.Length)];
    }

    public string GetRewardType()
    {
        return rewardType;
    }
}
