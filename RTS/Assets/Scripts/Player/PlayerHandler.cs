using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    [HideInInspector] public CameraController cameraController;

    [HideInInspector] public CharacterInput characterInput;
    
    
    private static PlayerHandler _instance;

    public static PlayerHandler PlayerHandlerInstance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<PlayerHandler>();
            }

            return _instance;
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        cameraController = GetComponent<CameraController>();
    }

    private void OnDisable()
    {
        cameraController.UnSubscribe(characterInput);
    }
}
