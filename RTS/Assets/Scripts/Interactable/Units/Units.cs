using System;
using System.Collections;
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
        
        public string nameOfUnit;
        private NavMeshAgent agent;
        private RaycastHit hit;
        private Coroutine thisCoolNewCoorutine;
        

        private void Start()
        {
            PlayerSelectedUnits.selectableUnits.Add(gameObject);
            Debug.Log(PlayerSelectedUnits.selectableUnits.Count);
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = minRangeToAttack - 3;
        }

        public virtual void OnClicked()
        {
            Debug.Log("This is the unit: " + nameOfUnit);
            Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        }

        public void DeSelected()
        {
            Debug.Log("I have now been deselected");
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        }

        private void MoveToClick(bool hasClicked)
        {
            var found = CameraController.GetMousePosition(out hit); 
            if(thisCoolNewCoorutine != null)
                StopCoroutine(thisCoolNewCoorutine);
            if (found && hit.collider.GetComponent<Enemy>())
            {
                thisCoolNewCoorutine = StartCoroutine(MoveToTargetThenAttack(hit));
                
            }
            else if(hasClicked && !PlayerSelectedUnits.holdingDownButton && found)
            {
                MoveToTarget(hit);
            }
        }

        private void MoveToTarget(RaycastHit hit)
        {
            agent.SetDestination(hit.point);
        }

        IEnumerator MoveToTargetThenAttack(RaycastHit hit2)
        {
            while (hit2.collider.GetComponent<Enemy>().hitPoints >= 0)
            {
                var distance = (transform.position - hit2.transform.position).sqrMagnitude;
                Debug.Log(distance);
                if (distance > minRangeToAttack)
                {
                    agent.destination = hit2.transform.position;
                }
                else
                {   
                    hit2.collider.GetComponent<Enemy>().hitPoints -= 10;
                    yield return new WaitForSeconds(0.1f);
                }   
                
                yield return new WaitForSeconds(0.01f);
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
