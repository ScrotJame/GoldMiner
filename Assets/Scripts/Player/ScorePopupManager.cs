using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScorePopupManager : MonoBehaviour
{
    public static ScorePopupManager instance;

    [SerializeField] private GameObject scorePopupPrefab;
    [SerializeField] private Canvas gameCanvas;

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
        }

        if (gameCanvas == null)
        {
            Debug.Log("Not find canvas");
            gameCanvas = FindObjectOfType<Canvas>();
        }
    }

    public void ShowScorePopup(int amount, Vector3 worldPosition)
    {
        if (scorePopupPrefab == null || gameCanvas == null)
        {
            return;
        }

        if (Camera.main == null)
        {
            return;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        GameObject popup = Instantiate(scorePopupPrefab, gameCanvas.transform);

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        
                Vector2 anchoredPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    gameCanvas.GetComponent<RectTransform>(),
                    screenPos,
                    gameCanvas.worldCamera,
                    out anchoredPos
                );
                popupRect.anchoredPosition = anchoredPos; 
            
        

        Text popupText = popup.GetComponentInChildren<Text>();
        if (popupText != null)
        {
            string prefix = amount >= 0 ? "+" : "";
            popupText.text = $"{prefix}{amount}";
            popupText.color = amount >= 0 ? Color.green : Color.red;
        }

        StartCoroutine(AnimatePopup(popup));
    }

    private IEnumerator AnimatePopup(GameObject popup)
    {
        float duration = 1.5f;
        float elapsed = 0f;

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = popup.AddComponent<CanvasGroup>();

        Vector3 startPos = popup.transform.position;
        Vector3 endPos = startPos + new Vector3(0, 1, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            popup.transform.position = Vector3.Lerp(startPos, endPos, t);
            if (t > 0.5f)
            {
                canvasGroup.alpha = 1 - ((t - 0.5f) * 2);
            }

            yield return null;
        }

        Destroy(popup);
    }
}