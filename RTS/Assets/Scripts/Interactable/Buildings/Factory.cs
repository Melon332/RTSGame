using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interactable;
using Managers;
using Player;
using UnityEngine;
using TMPro;

public class Factory : Buildings
{
    private ObjectPool _objectPool;
    [HideInInspector] public GameObject currentUnitConstructing;
    public List<GameObject> unitQueue = new List<GameObject>(9);

    private bool isConstructingUnit;
    
    private Coroutine createUnitCoroutine;
    
    private Coroutine ConstructUnit;

    private Vector3 rallyPointPosition;
    
    private EnemyBuildingBaseState currentState;
    
    public readonly EnemyBuildingCheckUnitState checkForUnits = new EnemyBuildingCheckUnitState();
    public readonly EnemyBuildingStateConstructUnits constructUnits = new EnemyBuildingStateConstructUnits();

    [SerializeField] private GameObject spawnPosition;
    
    protected override void Start()
    {
        base.Start();
        _objectPool = FindObjectOfType<ObjectPool>();
        if (isEnemy)
        {
            InvokeRepeating(nameof(TransisitonToState),5,10);
        }
    }

    public void StartConstructing()
    {
        var positionToSpawnTextObject = new Vector3(transform.position.x, 3, transform.position.z);
        if (textObject == null)
        {
            textObject = Instantiate(floatingText, positionToSpawnTextObject, Quaternion.Euler(90, 0, 0),
            transform);
        }
        if (createUnitCoroutine == null)
        {
            createUnitCoroutine = StartCoroutine(CreateUnit(textObject));
        }
    }
    
    public void TransisitonToState()
    {
        currentState = checkForUnits;
        currentState.EnterState(this);
    }
    public void TransisitonToState(EnemyBuildingBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }


    private IEnumerator CreateUnit(GameObject textBox)
    {
        while (unitQueue.Any())
        {
            if (!isConstructingUnit)
            {
                //1. 
                yield return new WaitForSeconds(0.1f); 
                currentUnitConstructing = unitQueue[0];
                currentUnitConstructing.transform.position = transform.position; 
                if (isEnemy)
                {
                    currentUnitConstructing.GetComponent<Units>().isEnemy = true;
                    currentUnitConstructing.GetComponent<Units>().isSelectable = false;
                    currentUnitConstructing.transform.position = spawnPosition.transform.position;
                }
                currentUnitConstructing.SetActive(true);
                currentUnitConstructing.GetComponent<MeshRenderer>().enabled = false;
               isConstructingUnit = true;
               PlayerManager.Instance.MoneyPlayerHad = PlayerManager.Instance.AmountOfMoneyPlayerHas;
               textBox.SetActive(true);
            }
            else
            {
                var unitComponent = currentUnitConstructing.GetComponent<Units>();
                var buildingText = textObject.GetComponent<TextMeshPro>();
                while(unitComponent.hitPoints < unitComponent.maxHitPoints)
                {
                    //Checks hit points and does an equation to convert it into % and set it into a text box
                    unitComponent.hitPoints = Mathf.Clamp(unitComponent.hitPoints, 0, unitComponent.maxHitPoints); 
                    unitComponent.hitPoints += unitComponent.amountOfHpPerSecond;
                    var equation = (unitComponent.hitPoints / unitComponent.maxHitPoints) * 100;
                    buildingText.text = "Constructing: " + equation.ToString("F0") + "%";
                    if (!isEnemy)
                    {
                        if (PlayerManager.Instance.playerMoneyRemoval == null)
                        {
                            PlayerManager.Instance.playerMoneyRemoval =
                                PlayerManager.Instance.StartCoroutine(
                                    PlayerManager.Instance.RemoveMoney(unitComponent));
                        }
                    }

                    UIManager.Instance.DecreasePlayerMoney();
                    yield return new WaitForSeconds(0.2f);
                }
                if (rallyPointPosition != Vector3.zero)
                {
                    unitComponent.MoveForward(rallyPointPosition);
                }
                else
                {
                    if (!unitComponent.isEnemy)
                    {
                        var position = new Vector3(transform.position.x, transform.position.y,
                            transform.position.z + 5);
                        unitComponent.MoveForward(position);
                    }
                }
                //1. Makes the unit interactable
                //2. Sets the current unit to null
                //3. Unit queue index 0 removed
                //4. Removed text box
                //5. Update unit count
                unitComponent.hasBeenConstructed = true;
                unitComponent.ActivateAllMesh();
                if (unitComponent.isEnemy)
                {
                    EnemyManager.Instance.enemiesOnMap.Add(unitComponent);
                }
                currentUnitConstructing = null;
                isConstructingUnit = false;
                textBox.SetActive(false);
                PlayerManager.Instance.playerMoneyRemoval  = null;
                UIManager.Instance.UpdateUnitCount();
                unitQueue.RemoveAt(0);
                if (unitComponent.GetComponent<Harvester>())
                {
                    unitComponent.GetComponent<Harvester>().targetedSupplyStation = gameObject;
                    unitComponent.GetComponent<Harvester>().FindNearestSupplyDepo();
                }

                if (!unitQueue.Any())
                {
                    StopCoroutine(createUnitCoroutine);
                    createUnitCoroutine = null;
                    Destroy(textBox);
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private void SetRallyPoint(bool hasClicked)
    {
        if (!hasClicked) return;
        if (!PlayerInputMouse.IsPointerOverUIObject())
        {
            if (BuildingManager.Instance.wantsToSetRallyPoint)
            {
                if (!PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out var hit)) return;
                if (!hit.collider.CompareTag("Ground")) return;
                rallyPointPosition = hit.point;
            }
        }
    }

    private void RemoveRallyPoint(bool hasLeftClicked)
    {
        rallyPointPosition = Vector3.zero;
    }

    public override void OnClicked()
    {

        base.OnClicked();
        if (!PlayerManager.Instance.hasEnoughPower) return;
        if (hasFinishedBuilding)
        {
            UIManager.Instance.ShowPanels(true, 1);
            Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        }
}

    public override void OnDeselect()
    {
        base.OnDeselect();
        UIManager.Instance.ShowPanels(false, 1);
        UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        BuildingManager.Instance.wantsToSetRallyPoint = false;
    }

    public override void Subscribe(CharacterInput publisher)
    {
        if (isEnemy) return;
        base.Subscribe(publisher);
        publisher.hasClicked += SetRallyPoint;
    }

    public override void UnSubscribe(CharacterInput publisher)
    {
        if (isEnemy) return;
        base.UnSubscribe(publisher);
        publisher.hasClicked -= SetRallyPoint;
    }
}
