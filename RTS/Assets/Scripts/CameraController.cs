using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera rtsCamera;
    // Start is called before the first frame update
    void Start()
    {
        rtsCamera = GetComponent<Camera>(); 
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
            Ray ray = rtsCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Clicked " + hit.collider.gameObject.name);
            }
        }
    }
}
