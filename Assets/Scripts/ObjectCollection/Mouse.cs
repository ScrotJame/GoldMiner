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
        }

    

        private void Update()
        {
            if (_isMove == true)
            {
                _Move();
            }
            if (_isMove == false) { return; }
        }

        protected void _Move()
        {
            if (currentState == GoldState.Idle)
            {
                transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime * (movingRight ? 1 : -1));

                if ((movingRight && transform.position.x >= _end.x) || (!movingRight && transform.position.x <= _start.x))
                {
                    movingRight = !movingRight;
                    Vector3 newScale = transform.localScale;
                    newScale.x *= -1;
                    transform.localScale = newScale;
                }
            }
            else if (currentState == GoldState.BeingGrabbed && hook != null)
            {
                _isMove = false;  // Dừng di chuyển hoàn toàn
                transform.position = hook.position; // Giữ chuột cố định vào hook
            }
        }

        public void SetHook(Transform hookTransform)
        {
            hook = hookTransform;
        }

    }
