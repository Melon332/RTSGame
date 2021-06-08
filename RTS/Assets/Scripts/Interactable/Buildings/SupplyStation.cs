using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class SupplyStation : Factory
{

    public override void OnClicked()
    {
        base.OnClicked();
        UIManager.Instance.ShowPanels(true,2);
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        UIManager.Instance.ShowPanels(false,2);
    }
}
