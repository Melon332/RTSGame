using System;
using System.Collections;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Interactable
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class Units : Entity, IInteractable
    {
        //BULLET SPAWNING
        public float amountOfHpPerSecond;

        [Header("Unit Bullet Variables")] [SerializeField]
        protected GameObject bullet;

        [SerializeField] protected GameObject bulletSpawnPosition;
        [Header("Unit Attack Variables")] public float minRangeToAttack;
        public float attackTimer;
        public int damageAmount;

        public RaycastHit hit;
        private RaycastHit enemyHit;

        private Coroutine AttackAndMove;

        private bool hasSubscribed = false;

        private MeshRenderer[] meshes;

        protected override void Awake()
        {
            base.Awake();
            UnitManager.SelectableUnits.Add(gameObject);
        }

        protected override void Start()
        {
            base.Start();
            canBeAttacked = false;
            isSelectable = true;
            //Disable all the meshes to make the unit invisible
            meshes = GetComponentsInChildren<MeshRenderer>();
            if (!hasBeenConstructed)
            {
                foreach (var mesh in meshes)
                {
                    mesh.enabled = false;
                }
            }
        }

        public void ActivateAllMesh()
        {
            //Reactivates them when construction is completed
            foreach (var mesh in meshes)
            {
                mesh.enabled = true;
            }
        }

        protected virtual void ClickToDoAction(bool hasClicked)
        {
            if (hasClicked)
            {
                //Gets the mouse position whenever you click
                var found = PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out hit);
                var entityClicked = hit.collider.GetComponent<Entity>();

                if (!gameObject.activeSelf) return;
                if (!found) return;
                if (entityClicked && entityClicked.canBeAttacked)
                {
                    if (canAttack)
                    {
                        enemyHit = hit;
                        //Sets the coroutine variable to store it and stop it.
                        if (AttackAndMove == null)
                        {
                            AttackAndMove = StartCoroutine(MoveToTargetThenAttack());
                        }
                    }
                    else
                    {
                        MoveToTarget(hit);
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
            //Moves to target in the case that the target isn't a build or another unit
            if (target.collider.CompareTag("Units") || target.collider.CompareTag("Buildings")) return;
            if (agent == null) return;
            transform.Rotate(target.point);
            agent.SetDestination(target.point);
            agent.isStopped = false;
        }

        public void MoveForward(Vector3 position)
        {
            if (agent == null) return;
            transform.Rotate(position);
            agent.SetDestination(position);
            agent.isStopped = false;
        }

        protected virtual IEnumerator MoveToTargetThenAttack()
        {
            if (enemyHit.collider.GetComponent<Entity>())
            {
                while (!enemyHit.collider.GetComponent<Entity>().isDead)
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

                        transform.LookAt(enemyHit.transform.position);
                        agent.isStopped = true;
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
            bulletObject.GetComponent<Bullet>().damageAmount = damageAmount;
        }

        public override void OnClicked()
        {
            if (hasBeenConstructed)
            {
                base.OnClicked();
                Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
            }
        }

        public override void OnDeselect()
        {
            base.OnDeselect();
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
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

        public override void OnDisable()
        {
            if (!gameObject.activeSelf) return;
            base.OnDisable();
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
            agent = null;
        }
    }
}
