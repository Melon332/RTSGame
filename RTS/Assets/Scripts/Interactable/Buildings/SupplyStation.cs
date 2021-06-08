using System.Collections;
using System.Collections.Generic;
using Enums;
using Managers;
using UnityEngine;

public class SupplyStation : Factory
{

    public override void OnClicked()
    {
        if (hasFinishedBuilding)
        {
            base.OnClicked();
            UIManager.Instance.ShowPanels(true, 2);
            HUD.SetCursor(CursorStates.Select);
        }
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        UIManager.Instance.ShowPanels(false,2);
    }
}
