using UnityEngine;

[CreateAssetMenu(fileName="DataObject", menuName = "Configuration/DataObject")]
public class DataObject: ScriptableObject
{
    public string objectName;
    public float weight;
    public int point;
}
