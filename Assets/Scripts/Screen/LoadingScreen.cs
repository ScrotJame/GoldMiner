//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;

//public class LoadingScreen : MonoBehaviour
//{
//    public Image background1; 
//    public Image background2;  
//    public Slider loadingBar;  

//    private void Start()
//    {
//        StartCoroutine(LoadGameScene());
//    }

//    IEnumerator LoadGameScene()
//    {
//        StartCoroutine(FadeBackground());

//        AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene");
//        operation.allowSceneActivation = false;

//        while (!operation.isDone)
//        {
//            // Cập nhật thanh loading
//            loadingBar.value = operation.progress;

//            if (operation.progress >= 0.9f)
//            {
//                loadingBar.value = 1f;
//                operation.allowSceneActivation = true;
//            }

//            yield return null;
//        }
//    }

//    IEnumerator FadeBackground()
//    {
//        float duration = 2f;
//        float time = 0f;

//        while (true)
//        {
//            time += Time.deltaTime;
//            float alpha = Mathf.PingPong(time, duration) / duration;

//            background1.color = new Color(1, 1, 1, 1 - alpha);
//            background2.color = new Color(1, 1, 1, alpha);

//            yield return null;
//        }
//    }
//}
