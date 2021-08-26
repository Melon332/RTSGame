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

        public Coroutine AttackAndMove;

        private bool hasSubscribed = false;

        private MeshRenderer[] meshes;

        private EnemyUnitBaseState currentState;

        public readonly EnemyUnitMoveState moveState = new EnemyUnitMoveState();
        public readonly EnemyUnitAttackState attackState = new EnemyUnitAttackState();

        public NavMeshPath path;

        [HideInInspector] public RangeDetection DetectionRangeScript;
        [HideInInspector] public BoxCollider DetectionRange;

        [HideInInspector] public Entity unitToAttack;
        protected override void Start()
        {
            base.Start();
            if (isEnemy)
            {
                path = new NavMeshPath();
            }
        }

        private void Update()
        {
            if (isEnemy)
            {
                currentState.Update(this);
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

        private IEnumerator MoveToTargetThenAttack()
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
        public IEnumerator AggroAttack()
        {
            while (!unitToAttack.isDead && unitToAttack != null)
            {
                var distance = (transform.position - unitToAttack.transform.position).sqrMagnitude;
                if (distance > minRangeToAttack)
                {
                    agent.isStopped = false;
                    agent.destination = unitToAttack.transform.position;
                    yield return new WaitForSeconds(0.0001f);
                }
                else
                {

                    transform.LookAt(unitToAttack.transform.position);
                    agent.isStopped = true;
                    Attack();
                    yield return new WaitForSeconds(attackTimer);
                }
                if (AttackAndMove != null && unitToAttack.isDead)
                {
                    StopCoroutine(AttackAndMove);
                    AttackAndMove = null;
                    unitToAttack = null;
                    agent.isStopped = false;
                    TransisitonToState(moveState);
                }
                if (unitToAttack == null) yield break;
            }
        }

        private void Attack()
        {
            var bulletObject = Instantiate(bullet, bulletSpawnPosition.transform);
            bulletObject.GetComponent<Bullet>().Setup(bulletSpawnPosition.transform.forward);
            bulletObject.GetComponent<Bullet>().damageAmount = damageAmount;
            bulletObject.GetComponent<Bullet>().instigator = this;
        }

        public override void OnClicked()
        {
            if (hasBeenConstructed)
            {
                base.OnClicked();
                if (isEnemy) return;
                Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
            }
        }

        public override void OnDeselect()
        {
            base.OnDeselect();
            if (isEnemy) return;
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
            base.OnDisable();
            UnitManager.SelectableUnits.Remove(gameObject);
            if (!gameObject.activeSelf) return;
            if (!hasSubscribed) return;
            if (isEnemy) return;
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        }

        public void TransisitonToState(EnemyUnitBaseState state)
        {
            currentState = state;
            currentState.EnterState(this);
        }

        public override void OnEnable()
        {
            base.OnEnable();
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
            if (isEnemy)
            {
                EnemyManager.Instance.enemiesOnMap.Add(this);
                path = new NavMeshPath();
                canBeAttacked = true;
                TransisitonToState(moveState);
            }
            else
            {
                UnitManager.SelectableUnits.Add(gameObject);
            }
        }

        public void ActivateUnit()
        {
            hitPoints = maxHitPoints;
            hasBeenConstructed = true;
        }

        public override void OnHit(int damage, Entity instigator)
        {
            base.OnHit(damage, instigator);
            if (isEnemy) return;
            if (AttackAndMove == null && canAttack)
            {
                unitToAttack = instigator;
                AttackAndMove = StartCoroutine(AggroAttack());
            }
        }
    }
}
