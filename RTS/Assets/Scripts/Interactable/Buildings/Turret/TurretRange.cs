using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretRange : MonoBehaviour
{
    private Turret mainTurret;
    private Coroutine mainTurretAttackCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        mainTurret = GetComponentInParent<Turret>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ground")) return;
        if (!other.GetComponent<Entity>()) return;
        if (!mainTurret.isEnemy)
        {
            if (other.gameObject.GetComponent<Entity>().canBeAttacked &&
                !mainTurret.attackableEnemies.Contains(other.gameObject))
            {
                mainTurret.attackableEnemies.Add(other.gameObject);
                if (mainTurretAttackCoroutine == null)
                {
                    mainTurretAttackCoroutine = StartCoroutine(mainTurret.FireAtEnemies());
                }
            }
        }
        else
        {
            if (!other.gameObject.GetComponent<Entity>().canBeAttacked &&
                !mainTurret.attackableEnemies.Contains(other.gameObject))
            {
                mainTurret.attackableEnemies.Add(other.gameObject);
                if (mainTurretAttackCoroutine == null)
                {
                    mainTurretAttackCoroutine = StartCoroutine(mainTurret.FireAtEnemies());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        mainTurret.attackableEnemies.Remove(other.gameObject);
        if (!mainTurret.attackableEnemies.Any())
        {
            if (mainTurretAttackCoroutine != null)
            {
                StopCoroutine(mainTurretAttackCoroutine);
                mainTurretAttackCoroutine = null;
            }
        }
    }
}
