using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public Transform _transform;
    [SerializeField]
    private MeshRenderer render;
    public Vector2 center = new Vector2(0f,0f);
    
    private float xyRatio;
    private Camera _camera;

    public float bgScale = 1f;
    [SerializeField] private float yOffset;
    public float zOffset = 0f;

    [Tooltip("Moving Scale when the camera moves on")]
    public Vector2 bgScrollingSpeed = new Vector2(1,0);
    //private Vector2 _movingSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        if (_transform == null)
        {
            _transform = this.transform;
        }
        if (_transform.TryGetComponent(out MeshRenderer _render))
        {
            render = _render;
        }
        render.sortingOrder = -1;
        FitSize();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeOffset();
      //  render.material.mainTextureOffset += Vector2.right * Time.deltaTime;
    }
    
    public void FitSize()
    {
        if (render == null)
        {
            return;
        }
        xyRatio = render.material.mainTexture.width / render.material.mainTexture.height;

        Vector2 _size = new Vector2(_camera.orthographicSize * xyRatio * 2, _camera.orthographicSize  *2);
        
        transform.localScale = _size * bgScale;
        
    }

    public void ChangeOffset()
    {

        _transform.position = _camera.transform.position + Vector3.forward * (20+ zOffset) +  new Vector3(0, yOffset);
        float _x = 0;
        if (bgScrollingSpeed.x !=0)
        {
            _x = (_camera.transform.position.x - center.x) /  _transform.localScale.x * bgScrollingSpeed.x;
        }
        float _y = 0;
        if (bgScrollingSpeed.y != 0)
        {
            _y = (_camera.transform.position.y - center.y) /  _transform.localScale.y* bgScrollingSpeed.y;
        }
        render.material.mainTextureOffset = new Vector2(_x,_y);
    }
}
