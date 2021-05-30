using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Interactable
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class Units : Entities, IInteractable
    {
        private RaycastHit hit;
        
        private Coroutine AttackAndMove;


        protected override void Start()
        {
            base.Start();
            canBeAttacked = false;
        }

        public override void OnClicked()
        {
            Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        }

        public void DeSelected()
        {
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        }

        private void MoveToClick(bool hasClicked)
        {
            //Gets the mouse position whenever you click
            var found = PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out hit); 
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
            transform.LookAt(target.point);
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
                    enemyHit.collider.GetComponent<IDestructable>().OnHit(10);
                    yield return new WaitForSeconds(attackTimer);
                }
                yield return new WaitForSeconds(attackTimer);
            }
        }

        public override void Subscribe(CharacterInput publisher)
        {
            publisher.hasClicked += MoveToClick;
        }

        public override void UnSubscribe(CharacterInput publisher)
        {
            publisher.hasClicked -= MoveToClick;
        }
    }
}
