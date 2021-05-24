using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    public Action<Vector2> _cameraMovement;

    protected Vector2 PastCameraMovement;

    protected void CameraMovement(Vector2 cameraMovement)
    {
        if (PastCameraMovement != cameraMovement)
        {
            _cameraMovement?.Invoke(cameraMovement);
            PastCameraMovement = cameraMovement;
        }
    }
}
