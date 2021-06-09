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
    }
    public void CreateBuilding(int buildingIndex)
    {
        if(PlayerManager.Instance.hasBuildingInHand) return;
        PlayerManager.Instance.hasBuildingInHand = true;
        Instantiate(Buildings[buildingIndex]);
    }
}
