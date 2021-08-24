using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<Entity> enemiesOnMap = new List<Entity>();


    public void AddEnemyToList(Entity objectToAdd)
    {
        enemiesOnMap.Add(objectToAdd);
    }

    public void RemoveEnemyFromList(Entity objectToRemove)
    {
        enemiesOnMap.Remove(objectToRemove);
    }
}
