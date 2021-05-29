using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace Interactable
{
    public class Units : MonoBehaviour, IInteractable,ISubscriber
    {
        public int hitPoints;
        public float minRangeToAttack;
        public float attackTimer;
        
        public string nameOfUnit;
        [HideInInspector] public NavMeshAgent agent;
        private RaycastHit hit;
        private Coroutine AttackAndMove;
        

        public virtual void Start()
        {
            PlayerSelectedUnits.selectableUnits.Add(gameObject);
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = minRangeToAttack - 3;
        }

        public virtual void OnClicked()
        {
            Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        }

        public void DeSelected()
        {
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        }

        private void MoveToClick(bool hasClicked)
        {
            var found = PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out hit); 
            if(AttackAndMove != null)
                StopCoroutine(AttackAndMove);
            if (found && hit.collider.GetComponent<Enemy>())
            {
                AttackAndMove = StartCoroutine(MoveToTargetThenAttack(hit));
            }
            else if(hasClicked && !PlayerSelectedUnits.holdingDownButton && found)
            {
                MoveToTarget(hit);
            }
        }

        private void MoveToTarget(RaycastHit hit)
        {
            var isLookingAtTarget = (int)Vector3.Dot(transform.position, hit.point);
            Mathf.Abs(isLookingAtTarget);
            Debug.Log(isLookingAtTarget);
            if (isLookingAtTarget != 1)
            {
                transform.LookAt(hit.point);
            }
            else
            {
                agent.SetDestination(hit.point);   
            }
        }

        IEnumerator MoveToTargetThenAttack(RaycastHit hit2)
        {
            while (hit2.collider.GetComponent<Enemy>().hitPoints >= 0)
            {
                var distance = (transform.position - hit2.transform.position).sqrMagnitude;
                if (distance > minRangeToAttack)
                {
                    agent.destination = hit2.transform.position;
                }
                else
                {   
                    hit2.collider.GetComponent<Enemy>().hitPoints -= 10;
                    yield return new WaitForSeconds(attackTimer);
                }   
                
                yield return new WaitForSeconds(attackTimer);
            }
        }

        public void Subscribe(CharacterInput publisher)
        {
            publisher.hasClicked += MoveToClick;
        }

        public void UnSubscribe(CharacterInput publisher)
        {
            publisher.hasClicked -= MoveToClick;
        }
    }
}
