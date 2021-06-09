using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SupplyDepo : Entities
{
    public int amountOfMoneyInDepo;

    public List<MoneyPerBox> depoBoxesAvaliableList = new List<MoneyPerBox>();
    public List<Harvester> harvestersWorkingOnThisDepo = new List<Harvester>();

    public int moneyGivenPerSecond;
    public float moneyDelay = 1;

    protected float timer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        isSelectable = false;
        depoBoxesAvaliableList = GetComponentsInChildren<MoneyPerBox>().ToList();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Harvester>() && other.gameObject.GetComponent<Harvester>().wantsToCollectMoney && other.GetComponent<Harvester>().targetedDepo == gameObject)
        {
            var harvester = other.gameObject.GetComponent<Harvester>();
            if (!harvester.isCollectingMoney)
            {
                harvestersWorkingOnThisDepo.Add(harvester);
                harvester.isCollectingMoney = true;
            }

            foreach (var harvesters in harvestersWorkingOnThisDepo)
            {
                harvesters.AddMoneyToHarvester();
                if (harvesters.currentAmountOfMoney > harvesters.holdMaxAmountOfMoney && !harvesters.isReturningToSupplyStation)
                {
                    harvesters.GoBackToSupplyStation(harvesters.targetedSupplyStation.transform.position);
                    harvesters.isCollectingMoney = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Harvester>())
        {
            harvestersWorkingOnThisDepo.Remove(other.gameObject.GetComponent<Harvester>());
        }
    }
}