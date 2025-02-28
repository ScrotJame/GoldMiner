using UnityEngine;

public class LinePod : MonoBehaviour
{
    public Transform hook; 
    public Transform originPoint; 
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        if (lineRenderer != null && hook != null && originPoint != null)
        {
            
            lineRenderer.SetPosition(0, originPoint.position); 
            lineRenderer.SetPosition(1, hook.position); 
        }
    }
}
