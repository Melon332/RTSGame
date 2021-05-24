using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputMouse : CharacterInput
{
    // Update is called once per frame
    private void Update()
    {
        CameraMovement(new Vector2(Input.mousePosition.x,Input.mousePosition.y));
    }
}
