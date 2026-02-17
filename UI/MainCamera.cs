using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private static Camera _mainCamera;
    public static Camera Main()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
        return _mainCamera;
    }
    private void Awake()
    {
        _mainCamera = Camera.main;
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        canvases.ForEach(x =>
        {
            x.worldCamera = Main();
            x.sortingLayerName = "UI";
        });
    }
}
