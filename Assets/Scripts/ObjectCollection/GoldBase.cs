using UnityEngine;

public class GoldBase : MonoBehaviour
{
    public DataObject RewindObject;
    public static GoldBase instanceGold;
    public GoldState currentState = GoldState.Idle;
    public int health = 1;
    public GameObject destroyEffect;
    public enum GoldState { Idle, BeingGrabbed, Collected }

    private Rigidbody2D rb2D;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public virtual void GotObj()
    {
        currentState = GoldState.BeingGrabbed;
        if (_animator != null)
        {
            _animator.SetBool("is_got", true);
            _animator.SetBool("is_got", false);
        }
    }

    public virtual void Collected()
    {
        currentState = GoldState.Collected;
        if (_animator != null)
        {
            _animator.SetBool("is_got", false);
            _animator.SetBool("is_got", true);
        }
    }
    private void OnDestroy()
    {
        LinePod linePod = FindObjectOfType<LinePod>();
        if (linePod != null)
        {
            linePod.ResetHook();
        }
    }
    public void TakeDamage()
    {
        health--;

        if (health <= 0)
        {
            DestroyObject();
        }
    }
    void DestroyObject()
    {
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
