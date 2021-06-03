using System;
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
        public float minRangeToAttack;
        public float attackTimer;
        
        private RaycastHit hit;
        
        private Coroutine AttackAndMove;
        
        private bool hasSubscribed = false;
        
        protected override void Start()
        {
            base.Start();
            UnitManager.SelectableUnits.Add(gameObject);
            canBeAttacked = false;
        }
        

        private void MoveToClick(bool hasClicked)
        {
            if (hasClicked)
            {
                //Gets the mouse position whenever you click
                var found = PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out hit);
                Debug.Log("Hello!");
                if (!gameObject.activeSelf) return;
                if (AttackAndMove != null && !hasFoundTarget)
                {
                    StopCoroutine(AttackAndMove);
                }

                if (found && hit.collider.GetComponent<Entities>() && hit.collider.GetComponent<Entities>().canBeAttacked && !hasFoundTarget)
                {
                    //Sets the coroutine variable to store it and stop it.
                    AttackAndMove = StartCoroutine(MoveToTargetThenAttack(hit));
                    Debug.Log("I have now found a target");
                    hasFoundTarget = true;
                }
                else if (!PlayerSelectedUnits.holdingDownButton && found)
                {
                    MoveToTarget(hit);
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
            hasFoundTarget = false;
        }

        IEnumerator MoveToTargetThenAttack(RaycastHit enemyHit)
        {
            while (enemyHit.collider.GetComponent<Entities>().hitPoints >= 0)
            {
                if (hasFoundTarget)
                {
                    var distance = (transform.position - enemyHit.transform.position).sqrMagnitude;
                    if (distance > minRangeToAttack)
                    {
                        agent.isStopped = false;
                        agent.destination = enemyHit.transform.position;
                    }
                    else
                    {
                        agent.isStopped = true;
                        transform.LookAt(enemyHit.transform.position);
                        Attack();
                        Debug.Log("I am now firing on target");
                    }
                    yield return new WaitForSeconds(attackTimer);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void Attack()
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

        public override void Subscribe(CharacterInput publisher)
        {
            if (hasSubscribed) return;
            publisher.hasClicked += MoveToClick;
            Debug.Log("test");
            hasSubscribed = true;
        }

        public override void UnSubscribe(CharacterInput publisher)
        {
            publisher.hasClicked -= MoveToClick;
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
