using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private CameraController _cameraController;

    private CharacterInput _characterInput;
    // Start is called before the first frame update
    void Awake()
    {
        _cameraController = GetComponent<CameraController>();
        _characterInput = GetComponent<CharacterInput>();
        Debug.Log(_characterInput);
        Debug.Log(_cameraController);
    }

    private void OnEnable()
    {
        _cameraController.Subscribe(_characterInput);
    }

    private void OnDisable()
    {
        _cameraController?.UnSubscribe(_characterInput);
    }
}
