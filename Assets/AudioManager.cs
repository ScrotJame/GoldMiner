using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource background;
    public AudioSource SFXsound;

    public AudioClip bgm1;
    public AudioClip bgm2;
    public AudioClip got;
    public AudioClip lost;
    public AudioClip boom;
    public AudioClip cashout1;
    public AudioClip cashout2;
        
    //UI sounds check
    public bool isSoundOn = true;
    public Button soundToggleButton;
    public Sprite soundOnIcon;
    public Sprite soundOffIcon;
    private void Start()
    {
        PlayRandomLoop();
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PlayRandomLoop()
    {
        AudioClip clipToPlay = Random.value < 0.7f ? bgm1 : bgm2;

        background.clip = clipToPlay;
        background.loop = true;
        background.Play();
        StartCoroutine(WaitAndPlayNext(clipToPlay.length));
    }
    public void PlayRandomCashout()
    {
        AudioClip clipToPlay = Random.value < 0.8f ? cashout1 : cashout2;

        if (SFXsound.isPlaying)
        {
            SFXsound.Stop(); 
        }

        SFXsound.clip = clipToPlay;
        SFXsound.Play(); 
    }

    private System.Collections.IEnumerator WaitAndPlayNext(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        PlayRandomLoop();
    }
    public void PlaySFX(AudioClip SFX)
    {
        SFXsound.PlayOneShot(SFX);
    }
    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;

        background.mute = !isSoundOn;
        SFXsound.mute = !isSoundOn;

        if (soundToggleButton != null)
        {
            Image btnImage = soundToggleButton.GetComponent<Image>();
            btnImage.sprite = isSoundOn ? soundOnIcon : soundOffIcon;
        }
    }
}
