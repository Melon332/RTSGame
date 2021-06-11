using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    
    public class UnitManager : MonoBehaviour
    {
        private static UnitManager _instance;

        public static UnitManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UnitManager>();
                }

                return _instance;

            }
        }

        public List<GameObject> buildableUnits = new List<GameObject>();
        
        public List<GameObject> selectedAttackingUnits = new List<GameObject>();
        public List<GameObject> selectedNonLethalUnits = new List<GameObject>();
        public static readonly List<GameObject> SelectableUnits = new List<GameObject>();
        
        private void Start()
        {
            
            UIManager.Instance.UpdateUnitCount();
        }
        public void BuildUnit(int unitIndex)
        {
            if (PlayerManager.Instance.AmountOfMoneyPlayerHas >= buildableUnits[unitIndex].GetComponent<Entity>().objectCost)
            {
                if (BuildingManager.Instance.currentSelectedBuilding.GetComponent<Factory>().unitQueue.Count < 9)
                {
                    BuildingManager.Instance.currentSelectedBuilding.GetComponent<Factory>().unitQueue
                        .Add(buildableUnits[unitIndex]);
                }
                BuildingManager.Instance.currentSelectedBuilding.GetComponent<Factory>().StartConstructing();
            }
            else
            {
                Debug.Log("Sorry, you need more money!");
            }
        }
    }
}

