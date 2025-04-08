using UnityEngine;

public class Mouse : GoldBase
{
    [SerializeField] private float _moveSpeed = 3f;
    public static Mouse Instance;
    private Vector3 _start, _end;
    private bool movingRight = true;
    private bool _isMove = true;
    private Transform hook;

    private void Start()
    {
        _start = transform.position + Vector3.left * 3;
        _end = transform.position + Vector3.right * 3;
        Instance = this;
    }

    private void Update()
    {
        if (_isMove)
        {
            _Move();
        }
        if (!_isMove) { return; }
    }

    protected void _Move()
    {
        if (currentState == GoldState.Idle)
        {
            transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime * (movingRight ? 1 : -1));

            if ((movingRight && transform.position.x >= _end.x) || (!movingRight && transform.position.x <= _start.x))
            {
                TurnAround();
            }
        }
        else if (currentState == GoldState.BeingGrabbed && hook != null)
        {
            _isMove = false;
            transform.position = hook.position; 
        }
    }

   
    private void TurnAround()
    {
        movingRight = !movingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Gold")|| (collision.CompareTag("TNT"))) && currentState == GoldState.Idle)
        {
            TurnAround(); 
        }
    }

    public void SetHook(Transform hookTransform)
    {
        hook = hookTransform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_start, _end);
    }
}