using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damageAmount;

    [SerializeField] private float bulletSpeed;

    private Vector3 shootDir;

    private void Start()
    {
        Destroy(gameObject,5f);
    }

    private void Update()
    {
        transform.position += shootDir * (bulletSpeed * Time.deltaTime);
    }

    public void Setup(Vector3 _shootDir)
    {
        shootDir = _shootDir;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Entity>())
        {
            other.GetComponent<Entity>().OnHit(damageAmount);
            Destroy(gameObject);
        }
    }
    
}
