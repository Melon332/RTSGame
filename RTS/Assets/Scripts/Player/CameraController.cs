using System;
using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class CameraController : MonoBehaviour, ISubscriber
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

    private void CameraMove(Vector2 directon)
    {
        Vector3 pos = _rtsCamera.transform.position;
        if (GameManager.Instance.KeyboardOrMouseCamera)
        {
            if (directon.y >= Screen.height - panBorderThickness)
            {
                pos.z += panSpeed * Time.deltaTime;
            }
            if (directon.y <= panBorderThickness)
            {
                pos.z -= panSpeed * Time.deltaTime;
            }
            if (directon.x <= panBorderThickness)
            {
                pos.x -= panSpeed * Time.deltaTime;
            }
            if (directon.x >= Screen.width - panBorderThickness)
            {
                pos.x += panSpeed * Time.deltaTime;
            }
        }
        else
        {
            
        }

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        transform.position = pos;
    }

    public void Subscribe(CharacterInput publisher)
    {
        publisher._cameraMovement += CameraMove;
    }

    public void UnSubscribe(CharacterInput publisher)
    {
        publisher._cameraMovement -= CameraMove;
    }
}
