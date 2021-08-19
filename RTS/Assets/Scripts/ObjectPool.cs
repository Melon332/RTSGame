using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject placeToStoreObjects;
    private readonly Dictionary<string, GameObject> pooledObjects = new Dictionary<string, GameObject>();

    [SerializeField] private int poolDepth;

    private readonly List<GameObject> storedObjects = new List<GameObject>();

    private void Awake()
    {
        pooledObjects.Add("Tank", UnitManager.Instance.buildableUnits[0]);
        pooledObjects.Add("Worker", UnitManager.Instance.buildableUnits[1]);
        pooledObjects.Add("Harvester", UnitManager.Instance.buildableUnits[2]);
        pooledObjects.Add("Factory", BuildingManager.Instance.BuildableBuildings[0]);
        pooledObjects.Add("SupplyStation", BuildingManager.Instance.BuildableBuildings[1]);
        pooledObjects.Add("PowerReactor", BuildingManager.Instance.BuildableBuildings[2]);
        pooledObjects.Add("Turret", BuildingManager.Instance.BuildableBuildings[3]);
        
        foreach (var objectToSpawn in pooledObjects)
        {
            for (int i = 0; i < poolDepth; i++)
            {
                GameObject go = Instantiate(objectToSpawn.Value,placeToStoreObjects.transform);
                storedObjects.Add(go);
                go.SetActive(false);
            }
        }
    }

    public GameObject GetAvaliableObject(string nameOfObject)
    {
        foreach (var objectToSearchFor in storedObjects)
        {
            if (objectToSearchFor.activeInHierarchy == false && objectToSearchFor.GetComponent<Entity>().nameOfUnit == nameOfObject)
            {
                return objectToSearchFor;
            }
        }
        return null;
    }
}
