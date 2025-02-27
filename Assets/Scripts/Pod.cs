using System;
using Unity.VisualScripting;
using UnityEngine;

public class Pod : MonoBehaviour
{
    private float _angle, _potion;
    private bool _flagCollect, _hasScored;
    private int _score, _count;
    private bool isAlive = true;
    private float flag = 0;

    public static Pod _Podinstance;
    private DataObject goldData;
    private Animator _anim;
    private AudioSource _audio;
    private AudioClip scrollClip, gotClip, useBoomClip;
    private Rigidbody2D _rigidbody2D;
    private Camera _mainCamera;
    private Vector3 _fistPosition;
    private Transform _transformPostion;

    [SerializeField] private float _scrollSpeed = 2.0f;
    [SerializeField] private float _rotationSpeed = 2;
    [SerializeField] private float _slow;

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
                Check();
                if (Mathf.Abs(transform.position.y) > 8 || Mathf.Abs(transform.position.x) > 8)
                {
                    _state = StateMoc._rewind;
                }
                break;
            case StateMoc._rewind:
                transform.Translate(Vector3.up * (_scrollSpeed - _slow) * Time.deltaTime);
                if (Mathf.Floor(transform.position.x) == Mathf.Floor(_fistPosition.x)
                    && Mathf.Floor(transform.position.y) == Mathf.Floor(_fistPosition.y))
                {
                    if (_transformPostion != null)
                    {
                        GoldBase gold = _transformPostion.GetComponent<GoldBase>();
                        _anim.SetBool("got", false);
                        Destroy(_transformPostion.gameObject);
                        _slow = 0;
                        _flagCollect = false;
                        _score += gold.RewindObject.point;
                        _count++;
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


    private void OnTriggerEnter2D(Collider2D collision)
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

            // Hiệu ứng animation
            Animator goldAnim = collision.gameObject.GetComponent<Animator>();
            if (goldAnim != null)
            {
                _anim.SetBool("got", true);
                goldAnim.SetBool("is_got", true);
            }
        }
    }

    private void Check()
    {
        Vector3 screenPosition = _mainCamera.WorldToViewportPoint(transform.position);
        if (screenPosition.y < 0 || screenPosition.y > 1|| screenPosition.x < 0 || screenPosition.x > 1)
        {
            _scrollSpeed = Mathf.Abs(_scrollSpeed);
            transform.position = _fistPosition;
            _state = StateMoc._rotation;
        }
    }
}