using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockMouse : MonoBehaviour
{
    void Start()
    {
        UnlockCursor();
    }

    void OnEnable()
    {
        UnlockCursor();
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}