using System;
using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class RangeDetection : MonoBehaviour
{
    private Units unit;
    [HideInInspector] public BoxCollider _collider;

    private void OnEnable()
    {
        _collider = GetComponent<BoxCollider>();
        unit = GetComponentInParent<Units>();
    }

    public void ChangeSizeOfRangeDetector(BoxCollider collider, int value)
    {
        collider.size = new Vector3(value / 2, 1, value / 2);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!unit.isEnemy) return;
        foreach (var playerUnits in PlayerManager.Instance.playerUnits)
        {
            if (other.gameObject == playerUnits.gameObject && playerUnits.hasBeenConstructed &&
                unit.AttackAndMove == null)
            {
                unit.unitToAttack = playerUnits;
                unit.TransisitonToState(unit.attackState);
            }
        }
    }
}
