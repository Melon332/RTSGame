using System;
using Enums;
using Player;
using UnityEditor;
using UnityEngine;

public class PlayerInputMouse : CharacterInput
{
    [SerializeField] float panBorderThickness = 50f;

    private KeyCode mouseButton = KeyCode.Mouse0;
    private readonly Vector3 sizeOfBox = new Vector3(50,0,50);
    
    // Update is called once per frame
    private void Update()
    {
        var direction = CameraDirection();
        CameraMovement(direction);
        HasClicked(Input.GetKeyDown(mouseButton));
        HasHeldDownButton(Input.GetMouseButton(0),MouseDirection());
        hasReleasedButton(Input.GetMouseButtonUp(0));
        HasLeftClickedMouseButton(Input.GetMouseButton(1));
        HasUsedMouseScrollWheel(Input.GetAxis("Mouse ScrollWheel"));
        Physics.Raycast(CameraController.rtsCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);
        IsMouseUnderAUnit(hit);
    }

    private Vector2 CameraDirection()
    {
        Vector2 direction = Vector2.zero;
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            direction.y += 1;
            HUD.SetCursor(CursorStates.PanUp);
        }

        else if (Input.mousePosition.y <= panBorderThickness)
        {
            direction.y -= 1;
            HUD.SetCursor(CursorStates.PanDown);
        }

        else if (Input.mousePosition.x <= panBorderThickness)
        {
            direction.x -= 1;
            HUD.SetCursor(CursorStates.PanLeft);
        }

        else if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            direction.x += 1;
            HUD.SetCursor(CursorStates.PanRight);
        }
        else if(PlayerSelectedUnits.hasSelectedUnits)
        {
            HUD.SetCursor(CursorStates.Move);
        }
        else
        {
            HUD.SetCursor(CursorStates.Select);
        }

        return direction;
    }

    private Vector2 MouseDirection()
    {
        return Input.mousePosition;
    }
    
}
