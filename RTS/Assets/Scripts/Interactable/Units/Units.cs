using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Interactable
{
    public class Units : MonoBehaviour, IInteractable,ISubscriber
    {
        public int hitPoints;
        public float minRangeToAttack;
        
        public string nameOfUnit;
        private NavMeshAgent agent;
        

        private void Start()
        {
            PlayerSelectedUnits.selectableUnits.Add(gameObject);
            agent = GetComponent<NavMeshAgent>();
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
            var found = CameraController.GetMousePosition(out RaycastHit hit);
            if (hasClicked && !PlayerSelectedUnits.holdingDownButton && found)
            {
                MoveToTarget(hit);
            }
        }

        private void MoveToAttack(bool hasClicked)
        {
            var found = CameraController.GetMousePosition(out RaycastHit hit);
            if (hasClicked && !PlayerSelectedUnits.holdingDownButton && found)
            {
                
            }
        }

        private void MoveToTarget(RaycastHit hit)
        {
            agent.SetDestination(hit.point);
        }

        IEnumerator MoveToTargetThenAttack(RaycastHit hit)
        {
            var distance = (transform.position - hit.transform.position).sqrMagnitude;
            while (distance > minRangeToAttack)
            {
                agent.SetDestination(hit.point);
                if (distance < minRangeToAttack)
                {
                    
                }
            }
        
            yield return new WaitForSeconds(1f);
            Debug.Log("I shoot pew pew at the guy!");
        }

        public void Subscribe(CharacterInput publisher)
        {
            publisher.hasClicked += MoveToClick;
            publisher.hasClicked += MoveToAttack;
        }

        public void UnSubscribe(CharacterInput publisher)
        {
            publisher.hasClicked -= MoveToClick;
        }
    }
}
