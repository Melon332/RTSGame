using System;
using System.Collections;
using System.Collections.Generic;
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

    public bool wantsToSetRallyPoint = false;


    public void SetRallyPoint()
    {
        wantsToSetRallyPoint = true;
    }
    public void CreateBuilding(int buildingIndex)
    {
        if (PlayerManager.Instance.AmountOfMoneyPlayerHas>= BuildableBuildings[buildingIndex].GetComponent<Entity>().objectCost )
        {
            if (PlayerManager.Instance.hasBuildingInHand) return;
            PlayerManager.Instance.hasBuildingInHand = true;
            Instantiate(BuildableBuildings[buildingIndex]);
        }
        else
        {
            Debug.Log("sorry, sir. You need more funds!");
        }
    }

    public void DestroyBuilding()
    {
        StartCoroutine(PlayerManager.Instance.AddMoney(currentSelectedBuilding.GetComponent<Entity>()));
        Destroy(currentSelectedBuilding);
        currentSelectedBuilding = null;
    }
}
