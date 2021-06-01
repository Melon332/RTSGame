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
            //Gets the mouse position whenever you click
            var found = PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out hit); 
            if (!gameObject.activeSelf) return;
            if(AttackAndMove != null)
                StopCoroutine(AttackAndMove);
            if (found && hit.collider.GetComponent<Entities>() && hit.collider.GetComponent<Entities>().canBeAttacked)
            {
                //Sets the coroutine variable to store it and stop it.
                AttackAndMove = StartCoroutine(MoveToTargetThenAttack(hit));
            }
            else if(hasClicked && !PlayerSelectedUnits.holdingDownButton && found)
            {
                MoveToTarget(hit);
            }
        }

        protected virtual void MoveToTarget(RaycastHit target)
        {
            if (target.collider.CompareTag("Units")) return;
            if (agent == null) return;
            transform.Rotate(target.point);
            agent.SetDestination(target.point);
            agent.isStopped = false;
        }

        IEnumerator MoveToTargetThenAttack(RaycastHit enemyHit)
        {
            while (enemyHit.collider.GetComponent<Entities>().hitPoints >= 0)
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
                    yield return new WaitForSeconds(attackTimer);
                }
                yield return new WaitForSeconds(attackTimer);
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
