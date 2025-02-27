using UnityEngine;

public class BackgroundScale : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Vector3 scale = transform.localScale;

        float spriteHeight = spriteRenderer.bounds.size.y;
        float spriteWidth = spriteRenderer.bounds.size.x;

        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Screen.width / Screen.height;

        scale.y = screenHeight / spriteHeight;
        scale.x = screenWidth / spriteWidth;
        transform.localScale = scale;
    }
}
