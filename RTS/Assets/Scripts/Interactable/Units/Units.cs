﻿using System;
using System.Collections;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Interactable
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class Units : Entities, IInteractable
    {
        [SerializeField] protected GameObject bullet;
        [SerializeField] protected GameObject bulletSpawnPosition;
        protected bool isFriendlyUnit = false;
        public float minRangeToAttack;
        public float attackTimer;
        
        private RaycastHit hit;
        private RaycastHit enemyHit;

        private Coroutine AttackAndMove;
        
        private bool hasSubscribed = false;
        
        protected override void Start()
        {
            base.Start();
            isFriendlyUnit = true;
            UnitManager.SelectableUnits.Add(gameObject);
            canBeAttacked = false;
        }
        

        protected virtual void ClickToDoAction(bool hasClicked)
        {
            if (hasClicked)
            {
                //Gets the mouse position whenever you click
                var found = PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out hit);
                var entityClicked = hit.collider.GetComponent<Entities>();


                if (!gameObject.activeSelf) return;
                if (!found) return;
                if (entityClicked && entityClicked.canBeAttacked)
                {
                    enemyHit = hit;
                    Debug.Log(enemyHit.collider.name);
                    //Sets the coroutine variable to store it and stop it.
                    if (AttackAndMove == null)
                    {
                        AttackAndMove = StartCoroutine(MoveToTargetThenAttack());
                    }
                }
                else if (!PlayerSelectedUnits.holdingDownButton && !PlayerInputMouse.IsPointerOverUIObject())
                {
                    MoveToTarget(hit);
                    enemyHit = new RaycastHit();
                    if (AttackAndMove != null)
                    {
                        StopCoroutine(AttackAndMove);
                        AttackAndMove = null;
                    }
                }
            }
        }

        protected virtual void MoveToTarget(RaycastHit target)
        {
            if (target.collider.CompareTag("Units")) return;
            if (agent == null || target.collider.GetComponent<Entities>()) return;
            transform.Rotate(target.point);
            agent.SetDestination(target.point);
            agent.isStopped = false;
        }

        protected virtual IEnumerator MoveToTargetThenAttack()
        {
            if (enemyHit.collider.GetComponent<Entities>())
            {
                while (enemyHit.collider.GetComponent<Entities>().hitPoints >= 0)
                {
                    var distance = (transform.position - enemyHit.transform.position).sqrMagnitude;
                    if (distance > minRangeToAttack)
                    {
                        agent.isStopped = false;
                        agent.destination = enemyHit.transform.position;
                        yield return new WaitForSeconds(0.0001f);
                    }
                    else
                    {
                        agent.isStopped = true;
                        transform.LookAt(enemyHit.transform.position);
                        Attack();
                        yield return new WaitForSeconds(attackTimer);
                    }
                }
            }

            if (AttackAndMove != null)
            {
                StopCoroutine(AttackAndMove);
                AttackAndMove = null;
            }
        }

        protected virtual void Attack()
        {
            var bulletObject = Instantiate(bullet, bulletSpawnPosition.transform);
            bulletObject.GetComponent<Bullet>().Setup(bulletSpawnPosition.transform.forward);
        }
        public override void OnClicked()
        {
            base.OnClicked();
            Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        }

        public override void OnDeselect()
        {
            base.OnDeselect();
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        }

        public override void OnHit(int damage)
        {
            base.OnHit(damage);
            if (hitPoints <= 0)
            {
                UnitManager.SelectableUnits.Remove(gameObject);
            }
        }

        public override void Subscribe(CharacterInput publisher)
        {
            if (hasSubscribed) return;
            publisher.hasClicked += ClickToDoAction;
            hasSubscribed = true;
        }

        public override void UnSubscribe(CharacterInput publisher)
        {
            publisher.hasClicked -= ClickToDoAction;
            hasSubscribed = false;
        }

        private void OnDisable()
        {
            if (!gameObject.activeSelf) return;
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
            agent = null;
        }
    }
}
