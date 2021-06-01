using System;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    public Action<Vector2> cameraMovement;
    public Action<bool> hasClicked;
    public Action<bool, Vector2> hasHeldDownButton;
    public Action<bool> hasReleasedButton;
    public Action<bool> hasLeftClickedMouse;
    public Action<float> mouseScrollWheel;
    public Action<RaycastHit> mousePosition;

    protected Vector2 PastCameraMovement;
    protected bool PastHasClicked;

    protected void CameraMovement(Vector2 _cameraMovement)
    {
        if (PastCameraMovement != _cameraMovement)
        {
            cameraMovement?.Invoke(_cameraMovement);
            PastCameraMovement = _cameraMovement;
        }
    }

    protected void HasClicked(bool _hasClicked)
    {
        if (PastHasClicked != _hasClicked)
        {
            hasClicked?.Invoke(_hasClicked);
            PastHasClicked = _hasClicked;
        }
    }

    protected void HasHeldDownButton(bool hasHeldDownMouseButton, Vector2 mousePos)
    {
        if (!PlayerManager.Instance.hasBuildingInHand)
        {
            hasHeldDownButton?.Invoke(hasHeldDownMouseButton, mousePos);
        }
    }

    protected void HasLeftClickedMouseButton(bool hasLeftClickedMouseButton)
    {
        hasLeftClickedMouse?.Invoke(hasLeftClickedMouseButton);
    }

    protected void HasUsedMouseScrollWheel(float scrollWheel)
    {
        mouseScrollWheel?.Invoke(scrollWheel);
    }

    protected void IsMouseUnderAUnit(RaycastHit mousePos)
    {
        mousePosition?.Invoke(mousePos);
    }
}
