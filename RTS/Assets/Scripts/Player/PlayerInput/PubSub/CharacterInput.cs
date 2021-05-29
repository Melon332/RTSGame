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
        hasHeldDownButton?.Invoke(hasHeldDownMouseButton, mousePos);
    }

    protected void HasReleaseButton(bool hasReleaseButton)
    {
        hasReleasedButton?.Invoke(hasReleaseButton);
    }

    protected void HasLeftClickedMouseButton(bool hasLeftClickedMouseButton)
    {
        hasLeftClickedMouse?.Invoke(hasLeftClickedMouseButton);
    }

    protected void HasUsedMouseScrollWheel(float scrollWheel)
    {
        mouseScrollWheel?.Invoke(scrollWheel);
    }
}
