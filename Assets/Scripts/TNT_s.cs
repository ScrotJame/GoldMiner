using UnityEngine;

public class TNT_s : GoldBase
{
    public float explosionRadius = 1.5f; 
    public float explosionForce = 500f;
    public LayerMask targetLayer;

    public GameObject explosionEffect;
    void Explode()
    {
        
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
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
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hook")) 
        {
            Explode();
        }
    }
}

