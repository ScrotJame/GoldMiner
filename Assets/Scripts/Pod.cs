﻿using System;
using UnityEngine;

public class Pod : MonoBehaviour
{
    private float _angle, _potion;
    private bool _flagCollect, _hasScored;
    public int _score, _count;
    private bool isAlive = true, _click = true;
    private float flag = 0;

    private Animator _anim, _animMiner;
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
        Time.timeScale = 1;
        _state = StateMoc._rotation;
        _animMiner = transform.root.GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        scrollClip = Resources.Load<AudioClip>("Sound/scroll");
        gotClip = Resources.Load<AudioClip>("Sound/got");
        useBoomClip = Resources.Load<AudioClip>("Sound/useBoom");
        _mainCamera = Camera.main;
        _fistPosition = transform.position;

        // Kiểm tra Animator
        if (_anim == null)
        {
            Debug.LogError("Animator component not found on Pod!");
        }
    }

    public void Awake()
    {
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
        if (Time.timeScale == 0) return;
        _HookGetObject();
    }

    protected virtual void _HookGetObject()
    {
        switch (_state)
        {
            case StateMoc._rotation:
                if (_animMiner != null) _animMiner.Play("MinerBaseState");
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    _click = false;
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
                _click = true;
                float tolerance = 0.1f;
                if (Mathf.Abs(transform.position.x - _fistPosition.x) < tolerance
                    && Mathf.Abs(transform.position.y - _fistPosition.y) < tolerance)
                {
                    _slow = 0;
                    _flagCollect = false;
                    if (_transformPostion != null)
                    {
                        GoldBase gold = _transformPostion.GetComponent<GoldBase>();
                        if (gold != null)
                        {
                            Destroy(_transformPostion.gameObject);
                            if (_anim != null) _anim.SetBool("got", false);
                            if (_animMiner != null) _animMiner.Play("MinerRewind");

                            int goldPoints = gold.RewindObject.point;
                            _score += goldPoints;
                            _count++;

                            if (ScoreControl.instance != null)
                            {
                                ScoreControl.instance.AddScore(goldPoints);
                                Debug.Log("Score added: " + goldPoints + ", Total: " + ScoreControl.instance.GetCurrentScore());
                            }
                            else
                            {
                                Debug.LogError("ScoreControl.instance is null!");
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
            Debug.Log("Got " + collision.gameObject.tag + "!");
            _state = StateMoc._rewind;
            _transformPostion = collision.gameObject.transform;
            _transformPostion.SetParent(transform);

            if (_flagCollect) return;
            _flagCollect = true;
            if (_animMiner != null) _animMiner.Play("MinerRoll");
            GoldBase gold = collision.gameObject.GetComponent<GoldBase>();
            if (gold != null)
            {
                if (gold.currentState == GoldBase.GoldState.Idle)
                {
                    gold.currentState = GoldBase.GoldState.BeingGrabbed;
                    gold.currentState = GoldBase.GoldState.Collected;
                }
            }

            GoldBase _gold = collision.gameObject.GetComponent<GoldBase>();
            if (_gold != null && _gold.RewindObject != null)
            {
                _slow = _gold.RewindObject.weight;
                Debug.Log($"Collected {collision.gameObject.tag}! Weight: {_gold.RewindObject.weight}, Points: {_gold.RewindObject.point}");
            }
            else
            {
                Debug.LogWarning("DataObject not found on " + collision.gameObject.name);
            }

            Animator goldAnim = collision.gameObject.GetComponent<Animator>();
            if (goldAnim != null)
            {
                if (_anim != null) _anim.SetBool("got", true);
                goldAnim.SetBool("is_got", true);
            }
        }
    }

    public void ResetHookPosition()
    {
        transform.position = _fistPosition;
        transform.rotation = Quaternion.identity;
        _state = StateMoc._rotation;
        _angle = 0;
        _slow = 0;
        _flagCollect = false;
        if (_transformPostion != null)
        {
            Destroy(_transformPostion.gameObject);
            _transformPostion = null;
        }
        if (_anim != null) _anim.SetBool("got", false);
        if (_animMiner != null) _animMiner.Play("MinerBaseState");
        Debug.Log("Hook reset to initial position!");
    }
}