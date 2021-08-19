using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    private static BuildingManager _instance;

    public static BuildingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BuildingManager>();
            }

            return _instance;
        }
    }
    
    public List<GameObject> BuildableBuildings = new List<GameObject>();
    public GameObject currentSelectedBuilding;
    
    public Material canPlaceBuildingMaterial;
    public Material cantPlaceBuildingMaterial;
    public Material normalBuildingMaterial;

    public List<IPowerConsumption> powerBuildings = new List<IPowerConsumption>();

    public bool wantsToSetRallyPoint = false;
    private ObjectPool _objectPool;

    private void Awake()
    {
        var stuff = FindObjectsOfType<MonoBehaviour>().OfType<IPowerConsumption>();
        foreach (var powerConsumption in stuff)
        {
            powerBuildings.Add(powerConsumption);
        }

        _objectPool = FindObjectOfType<ObjectPool>();
    }


    public void SetRallyPoint()
    {
        wantsToSetRallyPoint = true;
    }
    public void CreateBuilding(string buildingName)
    {
        if (PlayerManager.Instance.AmountOfMoneyPlayerHas>= _objectPool.GetAvaliableObject(buildingName).GetComponent<Entity>().objectCost )
        {
            if (PlayerManager.Instance.hasBuildingInHand) return;
            PlayerManager.Instance.hasBuildingInHand = true;
            _objectPool.GetAvaliableObject(buildingName).SetActive(true);
        }
        else
        {
            Debug.Log("sorry, sir. You need more funds!");
        }
    }

    public void DestroyBuilding()
    {
        StartCoroutine(PlayerManager.Instance.AddMoney(currentSelectedBuilding.GetComponent<Entity>()));
        currentSelectedBuilding.SetActive(false);
        currentSelectedBuilding = null;
    }
}
