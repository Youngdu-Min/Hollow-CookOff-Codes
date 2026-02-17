using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public Transform[] points;
    LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = points.Length;
        // lineRenderer.colorGradient = Color.yellow;
    }

    void Update()
    {
        for (var i = 0; i < points.Length; ++i)
        {
            if (points[i] == null)
            {
                lineRenderer.enabled = false;
                return;
            }
            lineRenderer.SetPosition(i, new Vector2(points[i].position.x, points[i].position.y));
        }
    }

    void OnEnable()
    {
        lineRenderer.enabled = true;
    }

    void OnDisable()
    {
        lineRenderer.enabled = false;
    }
}
