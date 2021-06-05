using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class Workers : GroundUnits
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    

    public override void OnClicked()
    {
        base.OnClicked();
        UIManager.Instance.ShowBuildingsPanel(true);
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        UIManager.Instance.ShowBuildingsPanel(false);
    }

    public void MoveBackAfterCompletingBuilding(Vector3 position)
    {
        if (agent == null) return;
        transform.Rotate(position);
        agent.SetDestination(position);
        agent.isStopped = false;
    }
}
