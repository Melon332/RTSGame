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

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        unit = GetComponentInParent<Units>();
    }

    public void ChangeSizeOfRangeDetector(BoxCollider collider, int value)
    {
        collider.size = new Vector3(value, 1, value);
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var playerUnits in PlayerManager.Instance.playerUnits)
        {
            if (other.gameObject == playerUnits.gameObject && playerUnits.hasBeenConstructed)
            {
                unit.unitToAttack = playerUnits;
                unit.TransisitonToState(unit.attackState);
            }
        }
    }
}
