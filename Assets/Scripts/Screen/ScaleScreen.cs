using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScaleScreen : MonoBehaviour
{
    public float targetWidth = 1920f;
    public float targetHeight = 1080f;

    void Start()
    {
        float targetRatio = targetWidth / targetHeight;
        float screenRatio = (float)Screen.width / (float)Screen.height;

        Camera.main.orthographicSize = targetHeight / 200f;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = targetHeight / 200f;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = (targetHeight / 200f) * differenceInSize;
        }
    }
}