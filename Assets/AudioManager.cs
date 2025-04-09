using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource background;
    public AudioSource SFXsound;

    public AudioClip bgm1;
    public AudioClip bgm2;
    public AudioClip got;
    public AudioClip lost;
    public AudioClip boom;
    public AudioClip cashout1;
    public AudioClip cashout2;

    private void Start()
    {
        PlayRandomLoop();
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
        AudioClip clipToPlay = Random.value < 0.5f ? cashout1 : cashout2;
        SFXsound.PlayOneShot(clipToPlay);
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
}
