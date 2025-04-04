using System.Collections;
using UnityEngine;

public class TNT_s : GoldBase
{
    public float explosionRadius = 1.5f;
    public float explosionForce = 500f;
    public LayerMask targetLayer;

    public GameObject explosionEffect;
    public GameObject shockwavePrefab;  

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        if (shockwavePrefab != null)
        {
            GameObject shockwave = Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
            StartCoroutine(ExpandShockwave(shockwave, explosionRadius));
        }

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector2.zero, 0, targetLayer);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("TNT")) continue;

            Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = hit.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce);
            }

            GoldBase destroyable = hit.collider.GetComponent<GoldBase>();
            if (destroyable != null)
            {
                destroyable.TakeDamage();
                StartCoroutine(destroyable.PlayFlameAnimationWithDelay());
            }
        }
    }

    IEnumerator ExpandShockwave(GameObject shockwave, float targetRadius)
    {
        float duration = 0.35f;  
        float startSize = 0.1f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(startSize, targetRadius, time / duration);
            shockwave.transform.localScale = new Vector3(scale, scale, 1);
            yield return null;
        }

        Destroy(shockwave);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hook"))
        {
            Explode();
        }
    }
}
