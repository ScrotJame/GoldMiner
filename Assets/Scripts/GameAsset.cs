using UnityEngine;

public class GameAsset : MonoBehaviour
{
    private static GameAsset _i;
    public static GameAsset i {
        get { 
        if (_i == null) _i = (Instantiate(Resources.Load("GameAsset")) as GameObject).GetComponent<GameAsset>();
        { return _i; } 
        } 
    }
    public Sprite Dynamite;
    public Sprite Drug;
    public Sprite Luck;
    public Sprite Streng;
    public Sprite Clock;
}
