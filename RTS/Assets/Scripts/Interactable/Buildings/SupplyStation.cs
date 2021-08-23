using System.Collections;
using System.Collections.Generic;
using Enums;
using Interactable;
using Managers;
using UnityEngine;

public class SupplyStation : Factory
{
    protected override void Start()
    {
        base.Start();
        selectionBox.transform.localScale = gameObject.transform.localScale * 4;
    }

    public override void OnClicked()
    {

        base.OnClicked();
        if (hasFinishedBuilding)
        {
            if (!PlayerManager.Instance.hasEnoughPower) return;
            UIManager.Instance.ShowPanels(true, 2);
            UIManager.Instance.ShowPanels(false,1);
            HUD.SetCursor(CursorStates.Select);
        }
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        UIManager.Instance.ShowPanels(false, 2);
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.GetComponent<Harvester>() && other.gameObject.GetComponent<Harvester>().targetedSupplyStation == gameObject)
        {
            other.gameObject.GetComponent<Harvester>()
                .StartCoroutine(other.gameObject.GetComponent<Harvester>().AddMoneyToPlayer());
        }
    }

    public void SpawnHarvester()
    {
        var objectPool = FindObjectOfType<ObjectPool>();
        var harvesterFound = objectPool.GetAvaliableObject("Harvester").GetComponent<Harvester>();
        harvesterFound.gameObject.SetActive(true);
        harvesterFound.ActivateUnit();
        harvesterFound.ActivateAllMesh();
        harvesterFound.transform.position = transform.position;
        harvesterFound.targetedSupplyStation = gameObject;
        harvesterFound.FindNearestSupplyDepo();
    }
}
