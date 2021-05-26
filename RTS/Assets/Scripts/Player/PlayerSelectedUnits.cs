using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;

public class PlayerSelectedUnits : MonoBehaviour,ISubscriber
{
    private Ray ray;
    private Camera _rtsCamera;
    // Start is called before the first frame update
    void Start()
    {
        _rtsCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ClickedOnSomething(bool hasClicked)
    {
        if (hasClicked)
        {
            RaycastHit hit;
            ray = _rtsCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.blue, 5f);
                if (hit.collider.GetComponent<IInteractable>() != null)
                {
                    hit.collider.GetComponent<IInteractable>().OnClicked();
                }
            }
        }
    }

    public void Subscribe(CharacterInput publisher)
    {
        publisher.hasClicked += ClickedOnSomething;
    }

    public void UnSubscribe(CharacterInput publisher)
    {
        publisher.hasClicked -= ClickedOnSomething;
    }
}
