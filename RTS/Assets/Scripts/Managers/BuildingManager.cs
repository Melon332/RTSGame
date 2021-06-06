using System;
using System.Collections;
using System.Collections.Generic;
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
    
    public List<GameObject> Buildings = new List<GameObject>();
    public GameObject currentSelectedBuilding;
    
    public Material canPlaceBuildingMaterial;
    public Material cantPlaceBuildingMaterial;
    public Material normalBuildingMaterial;

    public bool wantsToSetRallyPoint = false;


    public void SetRallyPoint()
    {
        wantsToSetRallyPoint = true;
        Debug.Log(wantsToSetRallyPoint);
    }
    public GameObject CreateBuilding(int buildingIndex)
    {
        PlayerManager.Instance.hasBuildingInHand = true;
        var buildingToCreate = Instantiate(Buildings[buildingIndex]);
        return buildingToCreate;
    }
}
