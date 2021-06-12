using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;

public class TurretRange : MonoBehaviour
{
    private Turret mainTurret;
    Coroutine test;
    // Start is called before the first frame update
    void Start()
    {
        mainTurret = GetComponentInParent<Turret>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Units"))
        {
            if (other.gameObject.GetComponent<Entity>().canBeAttacked && !mainTurret.attackableEnemies.Contains(other.gameObject))
            {
                mainTurret.attackableEnemies.Add(other.gameObject);
                if(test == null)
                {
                    test = StartCoroutine(mainTurret.FireAtEnemies());
                    Debug.Log("other");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        mainTurret.attackableEnemies.Remove(other.gameObject);
        if (!mainTurret.attackableEnemies.Any())
        {
            if (test != null)
            {
                StopCoroutine(test);
                test = null;
            }
        }
    }
}
