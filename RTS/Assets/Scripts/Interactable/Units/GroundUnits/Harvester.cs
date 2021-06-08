using System;
using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

public class Harvester : Units
{

    [Space]
    [Header("Harvester unit variable")]
    //MONEY VARIABLES
    public int holdMaxAmountOfMoney;

    public int currentAmountOfMoney;
    public GameObject targetedDepo;

    [HideInInspector] public bool isCollectingMoney;
    [HideInInspector] public bool isReturningToSupplyStation = false;

    [HideInInspector] public bool wantsToCollectMoney = false;
    //STATION IT WAS CREATED FROM
    public GameObject targetedSupplyStation;

    private void Update()
    {
        IsCloseToSupplyDepo();
    }

    protected override void ClickToDoAction(bool hasClicked)
    {
        base.ClickToDoAction(hasClicked);
        if (hasClicked)
        {
            var supplyDepoHit = hit.collider.GetComponent<SupplyDepo>();
            if (supplyDepoHit && supplyDepoHit.amountOfMoneyInDepo >= 0)
            {
                MoveToCollectMoney(hit);
            }
        }
    }

    protected override void MoveToTarget(RaycastHit target)
    {
        base.MoveToTarget(target);
        targetedDepo = null;
    }

    private void MoveToCollectMoney(RaycastHit targetDepo)
    {
        if (agent == null) return;
        transform.Rotate(targetDepo.point);
        agent.SetDestination(targetDepo.point);
        targetedDepo = targetDepo.collider.gameObject;
        wantsToCollectMoney = true;
        isCollectingMoney = false;
        Debug.Log(wantsToCollectMoney);
        agent.isStopped = false;
    }

    public void GoBackToSupplyStation(Vector3 position)
    {
        if (agent == null) return;
        isReturningToSupplyStation = true;
        transform.Rotate(position);
        agent.SetDestination(position);
        agent.isStopped = false;
    }

    private void GiveMoneyToPlayer()
    {
        PlayerManager.Instance.AmountOfMoneyPlayerHas += currentAmountOfMoney;
        currentAmountOfMoney = 0;
    }

    private bool IsCloseToSupplyDepo()
    {
        if (!isReturningToSupplyStation) return false;
        var distance = Vector3.Distance(transform.position, targetedSupplyStation.transform.position);
        if (distance < 2)
        {
            if (targetedDepo != null)
            {
                wantsToCollectMoney = true;
                GoBackToSupplyStation(targetedDepo.transform.position);
                isReturningToSupplyStation = false;
                GiveMoneyToPlayer();
            }
            return true;
        }
        else
        {
            return false;
        }
        
        
    }
}
