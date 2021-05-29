using Enums;
using UnityEngine;

public class PlayerInputKeyboard : CharacterInput
{
    public string xMoveAxis = "Horizontal";
    public string yMoveAxis = "Vertical";
    // Update is called once per frame
    private void Update()
    {
        CameraMovement(new Vector2(Input.GetAxis(xMoveAxis),Input.GetAxis(yMoveAxis)));
        if (Input.GetAxis(yMoveAxis) > 0)
        {
            HUD.SetCursor(CursorStates.PanUp);
        }
        else if (Input.GetAxis(yMoveAxis) < 0)
        {
            HUD.SetCursor(CursorStates.PanDown);
        }
        else if (Input.GetAxis(xMoveAxis) > 0)
        {
             HUD.SetCursor(CursorStates.PanRight);
        }
        else if (Input.GetAxis(xMoveAxis) < 0)
        {
             HUD.SetCursor(CursorStates.PanLeft);
        }
    }
}
