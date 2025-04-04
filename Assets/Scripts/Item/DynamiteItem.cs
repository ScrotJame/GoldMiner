using System.Collections;
using UnityEngine;

public class DynamiteItem : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 direction;
    private Transform targetItem;
    private AudioClip explosionClip;
    private AudioSource audioSource;
    private Pod pod; 

    public void Initialize(Vector3 startPos, Vector3 targetPos, Transform itemToDestroy, Pod podScript)
    {
        transform.position = startPos;
        direction = (targetPos - startPos).normalized;
        targetItem = itemToDestroy;
        pod = podScript;
        audioSource = GetComponent<AudioSource>();
        explosionClip = Resources.Load<AudioClip>("Sound/useBoom");

        if (targetItem == null || pod == null)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (targetItem == null || pod == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, targetItem.position) > 10f)
        {
            pod.OnDynamiteFinished(false); 
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetItem != null && collision.transform == targetItem)
        {
            GoldBase gold = collision.gameObject.GetComponent<GoldBase>();
            Animator goldAnim = gold?.GetComponent<Animator>();

            if (audioSource != null && explosionClip != null)
            {
                audioSource.PlayOneShot(explosionClip);
            }

            if (goldAnim != null)
            {
                goldAnim.Play("flame_anim");
            }
            StartCoroutine(CheckAndDestroyAfterAnimation(goldAnim, collision.gameObject));
            pod.OnDynamiteFinished(true);
        }
    }

    IEnumerator CheckAndDestroyAfterAnimation(Animator anim, GameObject obj)
    {
        float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        Destroy(obj);
    }

}