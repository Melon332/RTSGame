using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputMouse : CharacterInput
{
    [SerializeField] float panBorderThickness = 50f;

    private KeyCode mouseButton = KeyCode.Mouse0;
    
    // Update is called once per frame
    private void Update()
    {
        var direction = CameraDirection();
        CameraMovement(direction);
        HasClicked(Input.GetKeyDown(mouseButton));
    }

    private Vector2 CameraDirection()
    {
        Vector2 direction = Vector2.zero;
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            direction.y += 1;
        }

        if (Input.mousePosition.y <= panBorderThickness)
        {
            direction.y -= 1;
        }

        if (Input.mousePosition.x <= panBorderThickness)
        {
            direction.x -= 1;
        }

        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            direction.x += 1;
        }

        return direction;
    }
}
