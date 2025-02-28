using UnityEngine;

public class Miner : Pod
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator _anim;
    private AudioSource _audio;
    private AudioClip scrollClip, gotClip, useBoomClip;

    private bool _flagCollect, _hasScored, _isMove = true;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        scrollClip = Resources.Load<AudioClip>("Sound/scroll");
        gotClip = Resources.Load<AudioClip>("Sound/got");
        useBoomClip = Resources.Load<AudioClip>("Sound/useBoom");
    }

    void Update()
    {
        _HookGetObject();
    }

    protected override void _HookGetObject()
    {
        Debug.Log("Current state: " + _state);
        switch (_state)
        {
            case StateMoc._rotation:
                _Rotation();
                break;
            case StateMoc._click:
                _Click();
                break;
            case StateMoc._rewind:
                _Rewind();
                break;
        }
    }

    private void _Rewind()
    {
        transform.Translate(Vector3.up * (_scrollSpeed - _slow) * Time.deltaTime);
        float tolerance = 0.1f;
        if (Mathf.Abs(transform.position.x - _fistPosition.x) < tolerance
            && Mathf.Abs(transform.position.y - _fistPosition.y) < tolerance)
        {
            if (_transformPostion != null)
            {
                //get position of gold
                GoldBase gold = _transformPostion.GetComponent<GoldBase>();
                Destroy(_transformPostion.gameObject);
                _anim.SetBool("not_got", true);
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
        else
        {
            _state = StateMoc._rotation;
            _anim.SetBool("is_got", false);
        }
    }

    private void _Rotation()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            _anim.SetTrigger("is_pull");
            _anim.SetBool("is_got", false);
            _state = StateMoc._click;
        }
    }

    private void _Click()
    {
        _anim.SetBool("is_got", true);
        _isMove = false;
        //_anim.SetTrigger("is_pull");
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Gold"))
        {
            _anim.SetBool("not_got", true);
            _anim.SetTrigger("is_pull");

            if (_flagCollect) return;
            _flagCollect = true;
        }
        else if (other.CompareTag("Rock"))
        {
            _anim.SetTrigger("GotRock");
        }
        else
        {
            _anim.SetBool("not_got", false);
        }
    }
}