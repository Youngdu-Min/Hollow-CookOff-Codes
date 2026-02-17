using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MainCanvas : MonoBehaviour
{
    private static MainCanvas _mainCanvas;
    public static MainCanvas Instance()
    {
        if (_mainCanvas == null)
        {
            var obj = new GameObject("Main Canvas");
            obj.AddComponent<MainCanvas>();
        }

        return _mainCanvas;

    }

    private void Awake()
    {
        if (_mainCanvas == null)
        {
            _mainCanvas = this;
        }

        if (_mainCanvas != this)
        {
            Destroy(this);
            return;
        }
        Initiate();
    }
    private void Initiate()
    {
        _mainCanvas = this;
        if (TryGetComponent(out Canvas canvas))
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        WeaponSelectUI.Instance().transform.SetParent(_mainCanvas.transform);
        WeaponSelectUI.Instance().transform.localPosition = Vector2.zero;
        PirouetteGage.Instance().transform.SetParent(_mainCanvas.transform);
        AirBornePiUI.Instance().transform.SetParent(_mainCanvas.transform);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
