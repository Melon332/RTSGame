using System;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Interactable
{
    public class Units : MonoBehaviour, IInteractable,ISubscriber
    {
        public int hitPoints;
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
            Subscribe(FindObjectOfType<PlayerInputMouse>());
        }

        public void DeSelected()
        {
            Debug.Log("I have now been deselected");
            UnSubscribe(FindObjectOfType<PlayerInputMouse>());
        }

        private void MoveToClick(bool hasClicked)
        {
            if (hasClicked && !PlayerSelectedUnits.holdingDownButton)
            {
                agent.SetDestination(CameraController.GetMousePosition());
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
