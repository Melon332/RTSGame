using System.Collections;
using System.Collections.Generic;
using Enums;
using Managers;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private UIManager _uiManager;
    // Start is called before the first frame update
    void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();
    }

    public static void SetCursor(CursorStates states)
    {
        UIManager.SetCursorState((int)states);
    }
}
