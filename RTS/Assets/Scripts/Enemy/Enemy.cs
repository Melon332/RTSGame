using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class Enemy : Units
{
    public override void OnClicked()
    {
        Debug.Log("This is an enemy, you cannot select it");
    }
}
