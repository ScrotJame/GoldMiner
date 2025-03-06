using UnityEngine;

public class BackgroundScale : MonoBehaviour
{
    void Start()
    {
        ScaleToScreen();
    }

    void ScaleToScreen()
    {
        //SpriteRenderer sr = GetComponent<SpriteRenderer>();

        //if (sr == null)
        //{
        //    Debug.LogError("Không tìm thấy SpriteRenderer trên GameObject!");
        //    return;
        //}

        //float width = sr.bounds.size.x;
        //float height = sr.bounds.size.y;

        //float worldScreenHeight = Camera.main.orthographicSize * 3.5f;
        //float worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

        //Vector3 scale = transform.localScale;
        //scale.x = worldScreenWidth / width;
        //scale.y = worldScreenHeight / height;

        //transform.localScale = scale;
    }
}
