using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    public Action<Vector2> cameraMovement;
    public Action<bool> hasClicked;

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
}
