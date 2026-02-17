using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices; // DllImport

public class MouseMovement : MonoBehaviour
{
    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int x, int y);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
 private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);

    public enum MouseEventFlag : int
    {
        Absolute = 0x8000,
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        Move = 0x0001,
        RightDown = 0x0008,
        RightUp = 0x0010,
        Wheel = 0x0800,
        XDown = 0x0080,
        XUp = 0x0100,
        HWheel = 0x1000,
    }

    public static void Move(Vector2 Point)
    {
       MouseEventFlag Flag = MouseEventFlag.Move;

        mouse_event((int)Flag, (int)Point.x, (int)-Point.y, 0, IntPtr.Zero);
    }
}
