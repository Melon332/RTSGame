using System;
using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float panSpeed;
    [SerializeField] float panBorderThickness = 10f;

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
        CameraMove();
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

    private void CameraMove()
    {
        Vector3 pos = _rtsCamera.transform.position;
        
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height-panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width-panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        transform.position = pos;
    }
}
