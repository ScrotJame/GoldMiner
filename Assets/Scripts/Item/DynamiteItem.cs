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
            Debug.LogWarning("Target item or Pod is null in DynamiteItem.Initialize! Destroying Dynamite.");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (targetItem == null || pod == null)
        {
            Debug.Log("Target item or Pod lost! Destroying Dynamite.");
            Destroy(gameObject);
            return;
        }

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, targetItem.position) > 10f)
        {
            Debug.Log("Dynamite exceeded max distance. Destroying.");
            pod.OnDynamiteFinished(false); // khong pha vat pham khi di
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetItem != null && collision.transform == targetItem)
        {
            Debug.Log("Dynamite hit item: " + collision.gameObject.name);
            Destroy(collision.gameObject);
            if (audioSource != null && explosionClip != null)
            {
                audioSource.PlayOneShot(explosionClip);
            }
            pod.OnDynamiteFinished(true); // pa huy vat pham
            Destroy(gameObject);
        }
    }
}