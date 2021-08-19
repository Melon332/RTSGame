using System.Collections;
using System.Collections.Generic;
using Interactable;
using Managers;
using UnityEngine;

public class Workers : GroundUnits
{
    [HideInInspector] public int targetedBuilding;

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

    protected override void ClickToDoAction(bool hasClicked)
    {
        base.ClickToDoAction(hasClicked);
        if (hasClicked)
        {
            var buildingClicked = hit.collider.GetComponent<Buildings>();
            if (buildingClicked && buildingClicked.isBuilding && !buildingClicked.hasPlacedBuilding)
            {
                ContinueBuilding(hit);
            }
            else if (buildingClicked && !buildingClicked.hasFinishedBuilding)
            {
                ContinueBuilding(hit);
            }
            else if (buildingClicked && buildingClicked.hasFinishedBuilding &&
                     buildingClicked.hitPoints <= maxHitPoints)
            {
                ContinueBuilding(hit);
            }
        }
    }

    protected override void MoveToTarget(RaycastHit target)
    {
        base.MoveToTarget(target);
        ClearBuildingID();
    }

    private void ContinueBuilding(RaycastHit target)
    {
        if (agent == null) return;
        transform.Rotate(target.point);
        agent.SetDestination(target.point);
        targetedBuilding = target.collider.gameObject.GetInstanceID();
        agent.isStopped = false;
    }
    
    public void MoveBackAfterCompletingBuilding(Vector3 position)
    {
        if (agent == null) return;
        transform.Rotate(position);
        agent.SetDestination(position);
        agent.isStopped = false;
    }

    public void ClearBuildingID()
    {
        targetedBuilding = 0;
    }
}
