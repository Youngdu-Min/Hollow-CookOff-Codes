using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class CameraGizmos : MonoBehaviour
{
    private static  Camera _mainCam;
    public static Camera MainCam {
        get {
            if (_mainCam == null)
                _mainCam = Camera.main;
            return _mainCam;
        }

        set { _mainCam = MainCam; }
    }

    public Vector2 orthoMin;

    public Vector2 orthoMax;

    private float orthX, orthY;



    private void Awake()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        // camsize = new Vector2(MainCam.pixelWidth, MainCam.pixelHeight ) /50;
        orthoMin = transform.position - MainCam.transform.position + MainCam.ScreenToWorldPoint(Vector2.zero);
        orthoMax = transform.position - MainCam.transform.position + MainCam.ScreenToWorldPoint(new Vector2(MainCam.pixelWidth, MainCam.pixelHeight));
        
        
        Gizmos.DrawLine(orthoMin, new Vector2(orthoMin.x,orthoMax.y));
        Gizmos.DrawLine(orthoMin, new Vector2(orthoMax.x, orthoMin.y));
        Gizmos.DrawLine(new Vector2(orthoMin.x, orthoMax.y), orthoMax);
        Gizmos.DrawLine(new Vector2(orthoMax.x, orthoMin.y), orthoMax);

    }
#endif
}
