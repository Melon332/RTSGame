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
        HasHeldDownButton(Input.GetMouseButton(0),MouseDirection());
        hasReleasedButton(Input.GetMouseButtonUp(0));
        HasLeftClickedMouseButton(Input.GetMouseButton(1));
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

    private Vector2 MouseDirection()
    {
        return Input.mousePosition;
    }
}
