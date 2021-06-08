using System.Collections;
using System.Collections.Generic;
using Interactable;
using Player;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Units
{
    protected override void Start()
    {
        canBeAttacked = true;
        selectionBox = GetComponentInChildren<SelectionBox>().gameObject;
        agent = GetComponent<NavMeshAgent>();
        if (selectionBox != null)
        {
            selectionBox.SetActive(false);
        }

        isSelectable = false;
    }

    protected override void ClickToDoAction(bool hasClicked)
    {
        
    }

    public override void OnClicked()
    {
        if (PlayerManager.Instance.hasSelectedUnits) return;
        Debug.Log("This is an enemy, you cannot select it");
        base.OnClicked();
        Debug.Log(hitPoints);
    }
}
