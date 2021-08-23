using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

public class Turret : Buildings
{
    public List<GameObject> attackableEnemies = new List<GameObject>();

    [Header("Turret variables")] 
    [SerializeField] private int damageAmount;
    
    [SerializeField] private GameObject bullet;

    [SerializeField] private Transform bulletSpawnPosition;

    [SerializeField] private float attackTimer;

     private GameObject turretHead;

    [HideInInspector] public TurretRange turretRange;

     private Quaternion turretHeadOriginalRotation;

     public bool isEnemyTurret = false;
     
     

     protected override void Start()
    {
        base.Start();
        turretHead = transform.Find("TurretHead").gameObject;
        turretHeadOriginalRotation = turretHead.transform.rotation;
        turretRange = GetComponentInChildren<TurretRange>();
        if (!isEnemyTurret)
        {
            turretRange.gameObject.SetActive(false);
        }
    }

     public IEnumerator FireAtEnemies()
    {
        var randomTarget = Random.Range(0, attackableEnemies.Count);

        while (attackableEnemies.Any())
        {
            turretHead.transform.LookAt(attackableEnemies[randomTarget].transform.position);
            Attack();
            yield return new WaitForSeconds(attackTimer);
            if (attackableEnemies[randomTarget].GetComponent<Entity>().isDead)
            {
                attackableEnemies.Remove(attackableEnemies[randomTarget].gameObject);
                randomTarget = Random.Range(0, attackableEnemies.Count);
            }
        }

        turretHead.transform.rotation = turretHeadOriginalRotation;
    }
    private void Attack()
    {
        var bulletObject = Instantiate(bullet, bulletSpawnPosition.transform);
        bulletObject.GetComponent<Bullet>().Setup(bulletSpawnPosition.transform.forward);
        bulletObject.GetComponent<Bullet>().damageAmount = damageAmount;
    }

    private void ActivateTurret()
    {
        turretRange.gameObject.SetActive(true); 
    }

    public override void OnBuildingComplete()
    {
        base.OnBuildingComplete();
        ActivateTurret();
    }

    public override void Subscribe(CharacterInput publisher)
    {
        if (!isEnemyTurret)
        {
            base.Subscribe(publisher);
        }
    }

    public override void UnSubscribe(CharacterInput publisher)
    {
        if (!isEnemyTurret)
        {
            base.UnSubscribe(publisher);
        }
    }
}
