using UnityEngine;
using UnityEngine.UI;

public class ItemShopUI : MonoBehaviour
{

    [Header("Shop Events")]
    [SerializeField] GameObject shopUI;
    [SerializeField] Button nextMission;
    void Start()
    {
        _addShopEvent();
    }
    void _addShopEvent()
    {


        nextMission.onClick.RemoveAllListeners();
        nextMission.onClick.AddListener(_closeShopEvent);
    }

    void _openShopEvent() { 
        shopUI.SetActive(true);
    }
    void _closeShopEvent()
    {
        shopUI.SetActive(false);
    }
}
