﻿using UnityEngine;

public class PlayerInputKeyboard : CharacterInput
{
    public string xMoveAxis = "Horizontal";
    public string yMoveAxis = "Vertical";
    // Update is called once per frame
    private void Update()
    {
        CameraMovement(new Vector2(Input.GetAxis(xMoveAxis),Input.GetAxis(yMoveAxis)));
    }
}
