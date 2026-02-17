using System;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    private enum ScrollDirection { Up, Down, Left, Right }
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField] private ScrollDirection scrollDirection;
    [SerializeField] private bool isScrollSmoothly;
    private float currentScrollSpeed;

    void OnEnable()
    {
        currentScrollSpeed = isScrollSmoothly ? 0 : scrollSpeed;
    }

    void Update()
    {
        Vector2 dir;
        switch (scrollDirection)
        {
            case ScrollDirection.Up:
                dir = Vector2.up;
                break;
            case ScrollDirection.Down:
                dir = Vector2.down;
                break;
            case ScrollDirection.Left:
                dir = Vector2.left;
                break;
            case ScrollDirection.Right:
                dir = Vector2.right;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (isScrollSmoothly && currentScrollSpeed < scrollSpeed)
            currentScrollSpeed += scrollSpeed * Time.deltaTime;

        dir = new Vector2(dir.x * currentScrollSpeed * Time.deltaTime, dir.y * currentScrollSpeed * Time.deltaTime);
        transform.Translate(dir);
    }
}
