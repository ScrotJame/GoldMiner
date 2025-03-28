using UnityEngine;

public class LinePod : MonoBehaviour
{
    public static LinePod instance;
    public Transform hook;
    public Transform originPoint;
    private LineRenderer lineRenderer;
    private Transform targetGold = null;
    private Animator _anim;
    public GameObject dynamiteButton; 

    void Start()
    {
        instance = this; 
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = true;

        Material blackMaterial = new Material(Shader.Find("Sprites/Default"));
        blackMaterial.color = Color.black;
        lineRenderer.material = blackMaterial;

        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder = 5;
        _anim = hook.GetComponent<Animator>();
    }

    void Update()
    {
        if (lineRenderer != null && hook != null && originPoint != null)
        {
            lineRenderer.SetPosition(0, originPoint.position);

            if (targetGold == null)
            {
                lineRenderer.SetPosition(1, hook.position);
            }
            else
            {
                lineRenderer.SetPosition(1, targetGold.position);
            }
        }
    }

    public void ResetHook()
    {
        if (_anim != null)
        {
            _anim.SetBool("got", false); 
        }
    }

    
}