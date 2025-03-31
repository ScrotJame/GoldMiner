using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GoldBase;

public class Pod : MonoBehaviour
{
    public static Pod instance;
    private float _angle, _potion;
    private bool _flagCollect, _hasScored;
    public int _score, _count;
    private bool isMoving = true;

    private Animator _anim, _animMiner;
    private AudioSource _audio;
    private AudioClip scrollClip, gotClip, useBoomClip;
    private Rigidbody2D _rigidbody2D;
    private Camera _mainCamera;
    public Vector3 _fistPosition;
    public Transform _transformPostion;

    [SerializeField] public float _scrollSpeed = 2.0f;
    [SerializeField] public float _rotationSpeed = 1.25f;
    [SerializeField] public float _slow;

    private Animator _animHook;
    [SerializeField] private Transform minerTransform;

    [SerializeField] private GameObject dynamitePrefab;

    private bool isUsingDynamite = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return; 
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this == null || gameObject == null) return; 

        if (scene.name == "GamePlay")
        {
            gameObject.SetActive(true);
        }
        else if (scene.name == "Store")
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
        _state = StateMoc._rotation;

        if (minerTransform != null)
        {
            _animMiner = minerTransform.GetComponent<Animator>();
            Transform moc_0 = FindChildByName(minerTransform, "moc_0");
            if (moc_0 != null)
            {
                Transform hook = FindChildByName(moc_0, "Hook");
                if (hook != null) _animHook = hook.GetComponent<Animator>();
            }
            else
            {
                Debug.LogError("Không tìm thấy moc_0 trong miner!");
            }
        }
        else
        {
            Debug.LogError("Miner Transform không được gán trong Inspector!");
        }

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        scrollClip = Resources.Load<AudioClip>("Sound/scroll");
        gotClip = Resources.Load<AudioClip>("Sound/got");
        useBoomClip = Resources.Load<AudioClip>("Sound/useBoom");
        _mainCamera = Camera.main;
        _fistPosition = transform.position;
    }

    Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform found = FindChildByName(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    public enum StateMoc
    {
        _click = 1,
        _rotation = 2,
        _rewind = 3,
    }
    public StateMoc _state = StateMoc._rotation;

    void Update()
    {
        if (Time.timeScale == 0 || !isMoving) return;
        if (isMoving)
        {
            _HookGetObject();
        }
    }

    protected virtual void _HookGetObject()
    {
        switch (_state)
        {
            case StateMoc._rotation:
                if (_animMiner != null) _animMiner.Play("MinerBaseState");
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
                Vector3 viewportPos = _mainCamera.WorldToViewportPoint(transform.position);
                if (viewportPos.y < 0 || viewportPos.x < 0 || viewportPos.x > 1)
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
                    _slow = 0;
                    _flagCollect = false;
                    if (_transformPostion != null && !isUsingDynamite)
                    {
                        GoldBase gold = _transformPostion.GetComponent<GoldBase>();
                        if (gold != null)
                        {
                            Destroy(_transformPostion.gameObject);
                            if (_animHook != null) _animHook.SetBool("got", false);
                            if (_animMiner != null) _animMiner.Play("MinerRewind");

                            int goldPoints = gold.RewindObject.point;
                            _score += goldPoints;
                            _count++;

                            if (ScoreControl.instance != null)
                            {
                                ScoreControl.instance.AddScore(goldPoints);
                            }
                        }
                        _transformPostion = null;
                    }
                    _state = StateMoc._rotation;
                    if (_animMiner != null) _animMiner.Play("MinerBaseState");
                }
                break;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Gold") || collision.gameObject.CompareTag("TNT"))
        {
            _state = StateMoc._rewind;
            _transformPostion = collision.gameObject.transform;
            _transformPostion.SetParent(transform);

            if (_flagCollect) return;
            _flagCollect = true;

            if (_animMiner != null) _animMiner.Play("MinerRoll");

            Mouse mouse = collision.gameObject.GetComponent<Mouse>();
            GoldBase _gold = collision.gameObject.GetComponent<GoldBase>();

            if (mouse != null && mouse.RewindObject != null)
            {
                _slow = mouse.RewindObject.weight;
                mouse.currentState = GoldState.BeingGrabbed;
            }
            else if (_gold != null && _gold.RewindObject != null)
            {
                _slow = _gold.RewindObject.weight;
            }

            Animator mouseAnim = mouse?.GetComponent<Animator>();
            Animator goldAnim = _gold?.GetComponent<Animator>();

            if (mouseAnim != null || goldAnim != null)
            {
                if (_animHook != null) _animHook.SetBool("got", true);
                mouseAnim?.SetBool("is_got", true);
                goldAnim?.SetBool("is_got", true);
            }
        }
    }
    public void StopMovement()    { isMoving = false;    }

    public void ResumeMovement()    { isMoving = true;    }
    public void ResetHookPosition()
    {
        if (_transformPostion != null)
        {
            Mouse mouse = _transformPostion.GetComponent<Mouse>();
            if (mouse != null)
            {
                Destroy(mouse.gameObject);
            }
            _transformPostion = null;
        }

        transform.position = _fistPosition;
        transform.rotation = Quaternion.identity;
        _state = StateMoc._rotation;
        _angle = 0;
        _slow = 0;
        _flagCollect = false;

        if (_anim != null) _anim.SetBool("got", false);
        if (_animMiner != null) _animMiner.Play("MinerBaseState");
    }

    public void UseDynamite()
    {
        if (_transformPostion != null && _state == StateMoc._rewind)
        {
            if (dynamitePrefab == null)
            {
                Debug.LogError("Dynamite Prefab is not assigned in Inspector!");
                return;
            }
            isUsingDynamite = true;
            GameObject dynamite = Instantiate(dynamitePrefab, minerTransform.position, Quaternion.identity);
            DynamiteItem dynamiteScript = dynamite.GetComponent<DynamiteItem>();
            if (dynamiteScript != null)
            {
                dynamiteScript.Initialize(minerTransform.position, transform.position, _transformPostion, this);
                Debug.Log("Dynamite spawned from miner targeting: " + _transformPostion.name);
            }
            else
            {
                Debug.LogError("DynamiteItem script not found on Dynamite prefab!");
                Destroy(dynamite);
            }
            if (_animMiner != null) _animMiner.Play("usebom");
            if (_audio != null && useBoomClip != null)
            {
                _audio.PlayOneShot(useBoomClip);
            }
        }
        else
        {
            Debug.Log("No item to destroy with Dynamite or not in rewind state!");
        }
    }

    public void OnDynamiteFinished(bool itemDestroyed)
    {
        isUsingDynamite = false;
        if (itemDestroyed)
        {
            _slow = 0;
            _transformPostion = null;
            if (_animHook != null) _animHook.SetBool("got", false);
            if (_animMiner != null) _animMiner.Play("MinerRewind");
            Debug.Log("Dynamite destroyed item, _slow set to 0.");
        }
        else
        {
            Debug.Log("Dynamite did not destroy item.");
        }
    }
}