using System;
using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class CameraController : MonoBehaviour, ISubscriber
{
    [SerializeField] float panSpeed;
    private Vector2 cameraDirection;

    [SerializeField] private Vector2 panLimit;
    
    Camera _rtsCamera;

    private Ray ray;
    // Start is called before the first frame update
    void Start()
    {
        _rtsCamera = GetComponent<Camera>(); 
        ray = _rtsCamera.ScreenPointToRay(Input.mousePosition);
    }

    // Update is called once per frame
    void Update()
    {
        ClickedOnSomething();
        if (cameraDirection == Vector2.zero)
        {
            return;
        }
        MoveCamera(cameraDirection);
    }

    private void MoveCamera(Vector2 direction)
    {
        Vector3 pos = _rtsCamera.transform.position; 
        
        
        pos.z += direction.y * panSpeed * Time.deltaTime;
        pos.x += direction.x * panSpeed * Time.deltaTime;
                

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        transform.position = pos;
    }

    private void ClickedOnSomething()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            ray = _rtsCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawLine(ray.origin,hit.point,Color.blue,5f);
                if (hit.collider.GetComponent<IInteractable>() != null)
                {
                    hit.collider.GetComponent<IInteractable>().OnClicked();
                }
            }
        }
    }

    private void OnCameraMove(Vector2 direction)
    {
        cameraDirection = direction;
    }

    public void Subscribe(CharacterInput publisher)
    {
        publisher._cameraMovement += OnCameraMove;
    }

    public void UnSubscribe(CharacterInput publisher)
    {
        publisher._cameraMovement -= OnCameraMove;
    }
}
