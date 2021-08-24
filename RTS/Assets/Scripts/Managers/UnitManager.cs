using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    
    public class UnitManager : MonoBehaviour
    {
        private ObjectPool _objectPool;
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
            _objectPool = FindObjectOfType<ObjectPool>();
        }
        public void BuildUnit(string unitName)
        {
            if (PlayerManager.Instance.AmountOfMoneyPlayerHas >= _objectPool.GetAvaliableObject(unitName).GetComponent<Entity>().objectCost)
            {
                if (BuildingManager.Instance.currentSelectedBuilding.GetComponent<Factory>().unitQueue.Count < 9)
                {
                    BuildingManager.Instance.currentSelectedBuilding.GetComponent<Factory>().unitQueue
                        .Add(_objectPool.GetAvaliableObject(unitName));
                    _objectPool.GetAvaliableObject(unitName).GetComponent<Entity>().hasBeenPickedUpByPool = true;
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

