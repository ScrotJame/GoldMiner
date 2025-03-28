using UnityEngine;

[System.Serializable]
public class GameData
{
    public int item1Quantity;
}
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }
    private GameData gameData;
    private string Key = "GameData";
    private void Awake()
    {
     if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LoadData()
    {
        if (PlayerPrefs.HasKey(Key))
        {
            string json = PlayerPrefs.GetString(Key);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            gameData = new GameData();
            gameData.item1Quantity = 0; 
            SaveGameData();
        }
    }
    public void SaveGameData()
    {
        string json = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
    }
    public int GetItem1Quantity()
    {
        return gameData.item1Quantity;
    }
    public void SetItem1Quantity(int quantity)
    {
        gameData.item1Quantity = quantity;
        SaveGameData();
    }
    public void AddItem1Quantity(int amount)
    {
        gameData.item1Quantity += amount;
        SaveGameData();
    }
}
