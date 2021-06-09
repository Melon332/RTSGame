using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SupplyDepo : Entity
{
    public int amountOfMoneyInDepo;

    public List<MoneyPerBox> depoBoxesAvaliableList = new List<MoneyPerBox>();
    public List<Harvester> harvestersWorkingOnThisDepo = new List<Harvester>();

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
            if (!harvester.isCollectingMoney && !harvestersWorkingOnThisDepo.Contains(harvester))
            {
                harvestersWorkingOnThisDepo.Add(harvester);
                harvester.isCollectingMoney = true;
            }

            foreach (var harvesters in harvestersWorkingOnThisDepo)
            {
                if (harvesters.currentAmountOfMoney < harvesters.holdMaxAmountOfMoney && !harvesters.isReturningToSupplyStation)
                {
                    harvesters.AddMoneyToHarvester();
                }
                else
                {
                    harvesters.isCollectingMoney = false;
                    harvesters.GoBackToSupplyStation(harvesters.targetedSupplyStation.transform.position);
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
