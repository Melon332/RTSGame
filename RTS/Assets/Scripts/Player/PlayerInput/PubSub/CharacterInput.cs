using System;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    public Action<Vector2> cameraMovement;
    public Action<bool> hasClicked;
    public Action<bool, Vector2> hasHeldDownButton;

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

    protected void HasHeldDownButton(bool _hasHeldDownButton, Vector2 mousePos)
    {
        if (_hasHeldDownButton)
        {
            hasHeldDownButton?.Invoke(_hasHeldDownButton, mousePos);
        }
    }
}
