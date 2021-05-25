using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputMouse : CharacterInput
{
    [SerializeField] float panBorderThickness = 50f;
    
    // Update is called once per frame
    private void Update()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
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
        Debug.Log(direction);
        CameraMovement(direction);
    }
}
