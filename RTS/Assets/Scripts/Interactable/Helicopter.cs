using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class Helicopter : Units, IInteractable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClicked()
    {
        Debug.Log("I have been called " + name);
    }
}
