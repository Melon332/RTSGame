using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<Entity> enemiesOnMap = new List<Entity>();

    private static EnemyManager _instance;

    public static EnemyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnemyManager>();
            }

            return _instance;
        }
    }
    public void AddEnemyToList(Entity objectToAdd)
    {
        enemiesOnMap.Add(objectToAdd);
    }

    public void RemoveEnemyFromList(Entity objectToRemove)
    {
        enemiesOnMap.Remove(objectToRemove);
    }
}
