using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.AI;
namespace Interactable
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class Units : MonoBehaviour, IInteractable,ISubscriber
    {
        #region Unit Variables
        public int hitPoints;
        public float minRangeToAttack;
        public float attackTimer;
        public string nameOfUnit;
        #endregion

        [HideInInspector] public NavMeshAgent agent;
        private RaycastHit hit;
        
        private Coroutine AttackAndMove;
        

        public virtual void Start()
        {
            PlayerSelectedUnits.SelectableUnits.Add(gameObject);
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
            //Gets the mouse position whenever you click
            var found = PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out hit); 
            if(AttackAndMove != null)
                StopCoroutine(AttackAndMove);
            if (found && hit.collider.GetComponent<Enemy>())
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
        }

        IEnumerator MoveToTargetThenAttack(RaycastHit enemyHit)
        {
            while (enemyHit.collider.GetComponent<Enemy>().hitPoints >= 0)
            {
                var distance = (transform.position - enemyHit.transform.position).sqrMagnitude;
                if (distance > minRangeToAttack)
                {
                    agent.destination = enemyHit.transform.position;
                }
                else
                {   
                    enemyHit.collider.GetComponent<Enemy>().hitPoints -= 10;
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
