using System.Collections;
using System.Collections.Generic;
using Enums;
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
        if (hasFinishedBuilding)
        {
            base.OnClicked();
            UIManager.Instance.ShowPanels(true, 2);
            UIManager.Instance.ShowPanels(false,1);
            HUD.SetCursor(CursorStates.Select);
        }
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        UIManager.Instance.ShowPanels(false,2);
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
}
