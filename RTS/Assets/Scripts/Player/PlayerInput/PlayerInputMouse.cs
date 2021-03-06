using System.Collections.Generic;
using Enums;
using Player;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputMouse : CharacterInput
{
    private static bool IsMouseOverGameWindow => !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y);

    [SerializeField] float panBorderThickness = 50f;

    private const KeyCode MouseButton = KeyCode.Mouse0;

    // Update is called once per frame
    private void Update()
    {
        var direction = CameraDirection();
        CameraMovement(direction);
        HasClicked(Input.GetKeyDown(MouseButton));
        hasClickedAndShiftClicked(Input.GetKeyDown(MouseButton), Input.GetKey(KeyCode.LeftShift));
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
        if (!IsMouseInGameView()) return direction;
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
        else if (PlayerManager.Instance.hasSelectedUnits || PlayerManager.Instance.hasSelectedNonLethalUnits && !IsPointerOverUIObject())
        {
            HUD.SetCursor(CursorStates.Move);
        }
        else if (IsPointerOverUIObject())
        {
            HUD.SetCursor(CursorStates.Default);
        }
        else
        {
            HUD.SetCursor(CursorStates.Select);
        }
        if (IsMouseOverEnemy() && !PlayerManager.Instance.hasSelectedNonLethalUnits)
        {
            HUD.SetCursor(CursorStates.Attack);
        }
        if (IsMouseOverSupplyDepo())
        {
            HUD.SetCursor(CursorStates.Harvest);
        }
        return direction;
        
    }
    /// <summary>
    /// Checks if mouse is over an UI object
    /// </summary>
    /// <returns>1 if mouse is over UI Object 0 if mouse is not over an UI object</returns>
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static bool IsMouseInGameView()
    {
        return IsMouseOverGameWindow;
    }
/// <summary>
/// Checks if you have a lethal unit and is hovering over a enemy
/// </summary>
/// <returns>1 if enemy is under mouse cursor and has lethal unit. 0 if there is no enemy</returns>
    public static bool IsMouseOverEnemy()
    {
        if (!PlayerManager.Instance.hasSelectedUnits || !IsMouseInGameView()) return false;
        PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out var hit);
        var entity = hit.collider.GetComponent<Entity>();
        if (entity != null)
        {
            var hasFoundEnemy = entity.canBeAttacked;
            return hasFoundEnemy;
        }

        return false;
    }
/// <summary>
/// Checks if you have a harvester selected and are hovering over a supply depo
/// </summary>
/// <returns>1 if harvester selected and supply depo under mouse 0 if harvester not selected</returns>
    private static bool IsMouseOverSupplyDepo()
    {
        if (!PlayerManager.Instance.hasSelectedUnits && !PlayerManager.Instance.hasSelectedHarvester || !IsMouseInGameView()) return false;
        PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out var hit);
        bool hasHitSupplyDepo = hit.collider.GetComponent<SupplyDepo>() != null;
        if (PlayerManager.Instance.hasSelectedHarvester)
        {
            return hasHitSupplyDepo;
        }
        else
        {
            return false;
        }
    }
    private Vector2 MouseDirection()
    {
        return Input.mousePosition;
    }

}
