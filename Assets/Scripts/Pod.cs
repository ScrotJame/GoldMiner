using System;
using Unity.VisualScripting;
using UnityEngine;

public class Pod : MonoBehaviour
{
    private float _angle, _potion;
    private bool _flagCollect, _hasScored;
    public int _score, _count;
    private bool isAlive = true;
    private float flag = 0;

    public static Pod _Podinstance;
    private DataObject goldData;
    private Animator _anim;
        private AudioSource _audio;
        private AudioClip scrollClip, gotClip, useBoomClip;
    private Rigidbody2D _rigidbody2D;
    private Camera _mainCamera;
    public Vector3 _fistPosition;
    public Transform _transformPostion;

    [SerializeField] public float _scrollSpeed = 2.0f;
    [SerializeField] public float _rotationSpeed = 2;
    [SerializeField] public float _slow;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        scrollClip = Resources.Load<AudioClip>("Sound/scroll");
        gotClip = Resources.Load<AudioClip>("Sound/got");
        useBoomClip = Resources.Load<AudioClip>("Sound/useBoom");
        _mainCamera = Camera.main;
        _fistPosition = transform.position;

        
    }

    public void Awake()
    {
        _fistPosition = transform.position;
    }

    public enum StateMoc
    {
        _click,
        _rotation,
        _rewind,
    }
    public StateMoc _state = StateMoc._rotation;
    
    void Update()
    {
        if (Time.timeScale == 0) return;
        _HookGetObject();

    }
     protected virtual void _HookGetObject()
    {
        switch (_state)
        {
            case StateMoc._rotation:
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    _state = StateMoc._click;
                }
                _angle += _rotationSpeed;
                if (_angle >= 70 || _angle <= -70)
                {
                    _rotationSpeed *= -1;
                }
                transform.rotation = Quaternion.AngleAxis(_angle, Vector3.forward);
                break;
            case StateMoc._click:
                transform.Translate(Vector3.down * _scrollSpeed * Time.deltaTime);
                if (Mathf.Abs(transform.position.y) > 5 || Mathf.Abs(transform.position.x) > 9)
                {
                    _state = StateMoc._rewind;
                }
                break;
            case StateMoc._rewind:
                transform.Translate(Vector3.up * (_scrollSpeed - _slow) * Time.deltaTime);
                float tolerance = 0.1f; 
                if (Mathf.Abs(transform.position.x - _fistPosition.x) < tolerance
                    && Mathf.Abs(transform.position.y - _fistPosition.y) < tolerance)
                {
                    if (_transformPostion != null)
                    {
                        //get position of gold
                        GoldBase gold = _transformPostion.GetComponent<GoldBase>();
                        _anim.SetBool("got", false);
                        Destroy(_transformPostion.gameObject);
                        _slow = 0;
                        _flagCollect = false;
                        _score += gold.RewindObject.point;
                        _count++;

                        //set score -> count +1
                        if (GameControll.instance != null)
                        {
                            GameControll.instance._getScore(_score);
                            GameControll.instance._getNumber(_count);
                        }
                        Debug.Log("Current Score: " + _score);
                    }
                    transform.position = _fistPosition;
                    _state = StateMoc._rotation;
                }
                break;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Gold"))
        {
            Debug.Log("Got Gold!");

            _state = StateMoc._rewind;
            _transformPostion = collision.gameObject.transform;
            _transformPostion.SetParent(transform);

            if (_flagCollect) return;
            _flagCollect = true;

            // Lấy GoldBase từ đối tượng va chạm
            GoldBase gold = collision.gameObject.GetComponent<GoldBase>();
            if (gold != null)
            {
                if (gold.currentState == GoldBase.GoldState.Idle)
                {
                    gold.currentState = GoldBase.GoldState.BeingGrabbed;
                    gold.currentState = GoldBase.GoldState.Collected;
                }
            }

            // Lấy DataObject Gold vừa va chạm
            GoldBase _gold = collision.gameObject.GetComponent<GoldBase>();
            if (_gold != null && _gold.RewindObject != null)
            {
                _slow = _gold.RewindObject.weight;
                Debug.Log($"Collected Gold! Weight: {_gold.RewindObject.weight}, Points: {_gold.RewindObject.point}");

                if (GameControll.instance != null)
                {
                    GameControll.instance._getScore(_score);
                }
            }
            else
            {
                Debug.LogWarning("DataObject not found on " + collision.gameObject.name);
            }

            // Hiệu ứng animation của Gold
            Animator goldAnim = collision.gameObject.GetComponent<Animator>();
            Animator MineAnim= collision.gameObject.GetComponent<Animator>();
            if (goldAnim != null)
            {
                _anim.SetBool("got", true);
                goldAnim.SetBool("is_got", true);
            }
        }
    }

    
}

