using System;
using System.Collections;
using System.Collections.Generic;
using Interactable;
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

    [HideInInspector] public bool wantsToCollectMoney = false;
    
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
        agent.isStopped = false;
    }
    private void MoveToCollectMoney()
    {
        if (agent == null) return;
        transform.Rotate(targetedDepo.transform.position);
        agent.SetDestination(targetedDepo.transform.position);
        wantsToCollectMoney = true;
        isCollectingMoney = false;
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
        }
    }

    public IEnumerator AddMoneyToPlayer()
    {
        while (currentAmountOfMoney >= 0)
        {
            GiveMoneyToPlayer();
            yield return new WaitForSeconds(0.1f);
        }
        MoveToCollectMoney();
        yield return new WaitForSeconds(0.1f);
    }

    public void FindNearestSupplyDepo()
    {
        var depos = FindObjectsOfType<SupplyDepo>();
        foreach (var closestDepo in depos)
        {
            float distance = Vector3.Distance(transform.position, closestDepo.gameObject.transform.position);
            if (distance < 10)
            {
                targetedDepo = closestDepo.gameObject;
            }
            else
            {
                var randomNumber = Random.Range(0, depos.Length);
                targetedDepo = depos[randomNumber].gameObject;
            }
            MoveToCollectMoney();
        }
    }
}
