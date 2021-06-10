using System;
using System.Collections;
using System.Collections.Generic;
using Interactable;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

public class Harvester : Units
{

    [Space]
    [Header("Harvester unit variable")]
    //MONEY VARIABLES
    public int holdMaxAmountOfMoney;

    public int currentAmountOfMoney;
    public int moneyTakenPerSecond;
    public GameObject targetedDepo;

    [HideInInspector] public bool isCollectingMoney;
    [HideInInspector] public bool isReturningToSupplyStation = false;

    public bool wantsToCollectMoney = false;
    
    [SerializeField] protected int moneyDelay;

    protected float timer;
    
    //STATION IT WAS CREATED FROM
    public GameObject targetedSupplyStation;

    protected override void ClickToDoAction(bool hasClicked)
    {
        base.ClickToDoAction(hasClicked);
        if (hasClicked)
        {
            var supplyDepoHit = hit.collider.GetComponent<SupplyDepo>();
            var supplyStationHit = hit.collider.GetComponent<SupplyStation>();
            if (supplyDepoHit && supplyDepoHit.amountOfMoneyInDepo >= 0)
            {
                MoveToCollectMoney(hit);
            }
            else if(supplyStationHit && currentAmountOfMoney >= 0)
            {
                if (supplyStationHit.gameObject != targetedSupplyStation)
                {
                    targetedSupplyStation = supplyStationHit.gameObject;
                }
                GoBackToSupplyStation(targetedSupplyStation.transform.position);
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
        agent.isStopped = false;
    }
    private void MoveToCollectMoney()
    {
        if (agent == null) return;
        transform.Rotate(targetedDepo.transform.position);
        agent.SetDestination(targetedDepo.transform.position);
        wantsToCollectMoney = true;
        isCollectingMoney = false;
        isReturningToSupplyStation = false;
        agent.isStopped = false;
    }

    public void GoBackToSupplyStation(Vector3 position)
    {
        if (agent == null) return;
        isReturningToSupplyStation = true;
        transform.Rotate(position);
        agent.SetDestination(position);
        agent.isStopped = false;
        if (targetedDepo == null)
        {
            FindNearestSupplyDepo();
        }
        else
        {
            Debug.Log("I have target " + targetedDepo.name);
        }
    }

    private void GiveMoneyToPlayer()
    {
        PlayerManager.Instance.AmountOfMoneyPlayerHas++;
        currentAmountOfMoney--;
    }
    public void AddMoneyToHarvester()
    {
        timer += Time.deltaTime;
        Mathf.Clamp(currentAmountOfMoney, 0, holdMaxAmountOfMoney);
        if (timer >= moneyDelay)
        {
            timer = 0;
            currentAmountOfMoney += moneyTakenPerSecond;
            targetedDepo.GetComponent<SupplyDepo>().amountOfMoneyInDepo -= moneyTakenPerSecond;
        }
    }

    public IEnumerator AddMoneyToPlayer()
    {
        while (currentAmountOfMoney > 0)
        {
            GiveMoneyToPlayer();
            UIManager.Instance.UpdatePlayerMoney();
            yield return new WaitForSeconds(0.0001f);
        }
        MoveToCollectMoney();
        yield return new WaitForSeconds(0.0001f);
    }

    public SupplyDepo FindNearestSupplyDepo()
    {
        var depos = MapManager.Instance.GetAllSupplyDepos();
        var closestDistance = 25f;

        SupplyDepo target = null;
        var pos = transform.position;
        foreach (var closestDepo in depos)
        {
            float distance = Vector3.Distance(pos, closestDepo.gameObject.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = closestDepo;
                targetedDepo = closestDepo.gameObject;
            }
        }
        MoveToCollectMoney();

        if (target == null)
        {
            target = depos[Random.Range(0,depos.Length)];
        }

        return target;
    }

    public override void OnClicked()
    {
        base.OnClicked();
        PlayerManager.Instance.hasSelectedHarvester = true;
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        PlayerManager.Instance.hasSelectedHarvester = false;
    }
}
