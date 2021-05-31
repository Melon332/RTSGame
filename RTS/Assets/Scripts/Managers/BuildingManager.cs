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

    private void Start()
    {
        CreateBuilding(0);
    }

    public List<GameObject> Buildings = new List<GameObject>();
    
    public Material canPlaceBuilding;
    public Material cantPlaceBuilding;

    public GameObject CreateBuilding(int buildingIndex)
    {
        var buildingToCreate = Instantiate(Buildings[buildingIndex]);
        foreach (var _renderer in buildingToCreate.GetComponentsInChildren<MeshRenderer>())
        {
            _renderer.material = canPlaceBuilding;
        }
        return buildingToCreate;
    }
}
