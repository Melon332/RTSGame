using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interactable;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(BoxCollider))]
public class Buildings : Entity, IPowerConsumption
{
    public readonly List<GameObject> builders = new List<GameObject>();
    
    //BUILDING BOOL;
    private bool canPlace = true;
    [HideInInspector] public bool hasFinishedBuilding = false;
    [HideInInspector] public bool hasPlacedBuilding;

    [SerializeField] private Vector3 dropBuildingIntoFloor;

    [SerializeField] protected GameObject floatingText;

    private Vector3 targetToMoveBuilding = new Vector3(0, 0.5f, 0);

    [HideInInspector] public MeshRenderer[] buildingRenderer;
    private NavMeshObstacle buildingHitBox;
    private BoxCollider buildingCollider;
    [SerializeField] private float buildingSpeed;
    public float amountOfHpPerSecond;
    
    //Makes it easier to track the textobjects in the game
    [HideInInspector] public GameObject textObject;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (!isEnemy)
        {
            buildingRenderer = GetComponentsInChildren<MeshRenderer>();

            isBuilding = true;
            //References the obstacle collider for other agents to avoid
            buildingHitBox = GetComponent<NavMeshObstacle>();
            buildingHitBox.enabled = false;
            buildingHitBox.carving = false;

            buildingCollider = GetComponentInChildren<BoxCollider>();
            isSelectable = true;
        }
        selectionBox.transform.localScale = transform.localScale * 3;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        hitPoints = 0;
        if (isEnemy) return;
        //Incase that it is a finished building, remove power and check if the player has enough power
        if (PlayerHandler.PlayerHandlerInstance == null) return;
        if (PlayerHandler.PlayerHandlerInstance.characterInput != null)
        {
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
            if (hasFinishedBuilding)
            {
                PlayerManager.Instance.CheckIfPowerIsSufficient(costOfPower, true);
                UIManager.Instance.UpdateRequiredPowerText();
            }
            hasPlacedBuilding = false;
            hasFinishedBuilding = false;
            isDead = true;
        }

        //Destroys the text object incase it's being built.
        if (textObject != null)
        {
            Destroy(textObject);
        }

    }

    public override void Subscribe(CharacterInput publisher)
    {
        if (isEnemy) return;
        publisher.hasClicked += PlaceBuilding;
        publisher.mousePosition += CanPlaceBuilding;
        publisher.hasLeftClickedMouse += CancelBuilding;
    }

    public override void UnSubscribe(CharacterInput publisher)
    {
        if (isEnemy) return;
        publisher.mousePosition -= CanPlaceBuilding;
        publisher.hasLeftClickedMouse -= CancelBuilding;
        publisher.hasClicked -= PlaceBuilding;
    }

    public override void OnClicked()
    {
        if (!isEnemy)
        {
            if (hasFinishedBuilding)
            {
                //Will be used later
                // if (!PlayerManager.Instance.hasEnoughPower) return;
                //Checks if the player has units selected.
                if (PlayerManager.Instance.hasSelectedUnits || PlayerManager.Instance.hasSelectedNonLethalUnits)
                {
                    //Resets the units selection list so that the player can focus on the building
                    foreach (var lethalUnits in UnitManager.Instance.selectedAttackingUnits)
                    {
                        lethalUnits.GetComponent<Entity>().OnDeselect();
                    }

                    foreach (var nonLethalUnit in UnitManager.Instance.selectedNonLethalUnits)
                    {
                        nonLethalUnit.GetComponent<Entity>().OnDeselect();
                        PlayerManager.Instance.hasSelectedNonLethalUnits = false;
                    }

                    //Clears the list and tells the player it has no units selected.
                    PlayerManager.Instance.hasSelectedUnits = false;
                    PlayerManager.Instance.hasSelectedNonLethalUnits = false;
                    UnitManager.Instance.selectedAttackingUnits.Clear();
                    UnitManager.Instance.selectedNonLethalUnits.Clear();
                }

                HUD.SetCursor(CursorStates.Select);
            }
            else
            {
                HUD.SetCursor(CursorStates.Select);
                UnitManager.Instance.selectedNonLethalUnits.Remove(gameObject);
                PlayerManager.Instance.hasSelectedNonLethalUnits = false;
            }

            BuildingManager.Instance.currentSelectedBuilding = gameObject;
            UIManager.Instance.ShowPanels(true, 3);
        }
        base.OnClicked();
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        if (isEnemy) return;
        UIManager.Instance.ShowPanels(false, 3);
        BuildingManager.Instance.currentSelectedBuilding = null;
    }

    private void CanPlaceBuilding(RaycastHit mousePos)
    {
        if (hasFinishedBuilding) return;
        if(hasPlacedBuilding) return;
        transform.position = new Vector3(mousePos.point.x, 0, mousePos.point.z);
        if (canPlace && !PlayerInputMouse.IsPointerOverUIObject())
        {
            foreach (var buildingBlocks in buildingRenderer)
            {
                if (!buildingBlocks.GetComponent<SelectionBox>())
                {
                    buildingBlocks.material = BuildingManager.Instance.canPlaceBuildingMaterial;
                }
            }
        }
        else
        {
            foreach (var buildingBlocks in buildingRenderer)
            {
                if (!buildingBlocks.GetComponent<SelectionBox>())
                {
                    buildingBlocks.material = BuildingManager.Instance.cantPlaceBuildingMaterial;
                }
            }
        }
    }

    private void PlaceBuilding(bool place)
    {
        if (hasPlacedBuilding) return;
        if (place && canPlace && !PlayerInputMouse.IsPointerOverUIObject())
        {
            PlayerManager.Instance.hasBuildingInHand = false;
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);

            //Drop the building into to floor to rebuild it.
            var position = transform.position;
            dropBuildingIntoFloor.x = position.x;
            dropBuildingIntoFloor.z = position.z;
            position = dropBuildingIntoFloor;
            transform.position = position;
            buildingCollider.center = new Vector3(buildingCollider.center.x,Mathf.Abs(dropBuildingIntoFloor.y), buildingCollider.center.z);
            StartCoroutine(BuildBuilding());
            hasPlacedBuilding = true;

            foreach (var buildingBlocks in buildingRenderer)
            {
                if (buildingBlocks.GetComponent<SelectionBox>()) return;
                buildingBlocks.material = BuildingManager.Instance.normalBuildingMaterial;
            }

        }
        else
        {
            Debug.Log("Sorry sir, I cant build there");
        }
    }

    private IEnumerator BuildBuilding()
    {
        PlayerManager.Instance.MoneyPlayerHad = PlayerManager.Instance.AmountOfMoneyPlayerHas;
        UIManager.Instance.DecreasePlayerMoney();
        PlayerManager.Instance.playerUnits.Add(this);

        float step = buildingSpeed * Time.deltaTime;
        targetToMoveBuilding = new Vector3(transform.position.x, Mathf.Abs(dropBuildingIntoFloor.y), transform.position.z);
        var positionToSpawnTextObject = new Vector3(transform.position.x, 3f, transform.position.z);
        textObject = Instantiate(floatingText, positionToSpawnTextObject, Quaternion.Euler(90, 0, 0));
        var buildingText = textObject.GetComponent<TextMeshPro>();
        PlayerManager.Instance.playerMoneyRemoval = PlayerManager.Instance.StartCoroutine(PlayerManager.Instance.RemoveMoney(this));

        
        while (hitPoints < maxHitPoints)
        {
            if (isDead)
            {
                StopCoroutine(BuildBuilding());
                gameObject.SetActive(false);
            }

            while (builders.Count == 0) {
                yield return new WaitForSeconds (0.2f);
            }

            while (PlayerManager.Instance.AmountOfMoneyPlayerHas <= 0)
            {
                yield return new WaitForSeconds(0.2f);
            }

            hitPoints = Mathf.Clamp(hitPoints, 0, maxHitPoints);
            if (hitPoints <= maxHitPoints)
            {
                hitPoints += amountOfHpPerSecond;
                var equation = (hitPoints / maxHitPoints) * 100;
                Mathf.Clamp(equation, 0, 100);
                buildingText.text = "Building: " + equation.ToString("F0") + "%";
            }
            transform.position = Vector3.MoveTowards(transform.position, targetToMoveBuilding, step);
            yield return new WaitForSeconds(0.1f);
        }
        //Incase the hit points aren't maxed, max it out.
        hitPoints = maxHitPoints;
        //Enable colliders to make AI avoid buildings
        buildingCollider.isTrigger = false;
        buildingHitBox.enabled = true;
        buildingHitBox.carving = true;
        hasFinishedBuilding = true;
        OnBuildingComplete();
        hasBeenConstructed = true;
        PlayerManager.Instance.playerMoneyRemoval  = null;
        UIManager.Instance.UpdateRequiredPowerText();
        //Destroys the text box.
        Destroy(textObject);
        if (GetComponent<SupplyStation>())
        {
            GetComponent<SupplyStation>().SpawnHarvester();
        }

        //Moves the builder away from the building
        foreach (var builder in builders)
        {
            var worker = builder.GetComponent<Workers>();
            var transformPosition = builder.transform.position;
            var position = new Vector3(transformPosition.x, transformPosition.y, transformPosition.z);
            position.z += -5f;
            worker.MoveBackAfterCompletingBuilding(position);
            worker.ClearBuildingID();
            worker.agent.isStopped = false;
        }
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator RepairBuilding()
    {
        var positionToSpawnTextObject = new Vector3(transform.position.x, 3, transform.position.z);
        textObject = Instantiate(floatingText, positionToSpawnTextObject, Quaternion.Euler(90, 0, 0),
            transform);
        while (hitPoints <= maxHitPoints)
        {
            if (isDead)
            {
                StopCoroutine(RepairBuilding());
                gameObject.SetActive(false);
            }

            while (builders.Count == 0) {
                yield return new WaitForSeconds (0.2f);
            }

            hitPoints = Mathf.Clamp(hitPoints, 0, maxHitPoints);
            if (hitPoints < maxHitPoints)
            {
                hitPoints += amountOfHpPerSecond;
                var equation = (hitPoints / maxHitPoints) * 100;
                textObject.GetComponent<TextMeshPro>().text = "Repairing: " + equation.ToString("F0") + "%";
            }
            else
            {
                Destroy(textObject);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void CancelBuilding(bool hasClicked)
    {
        if (!hasPlacedBuilding)
        {
            if (!hasClicked) return;
            UIManager.Instance.DecreasePlayerMoney();
            UIManager.Instance.UpdatePlayerMoney();
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
            PlayerManager.Instance.hasBuildingInHand = false;
            gameObject.SetActive(false);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground")) return;
        canPlace = false;
        if (!other.CompareTag("Worker")) return;
        if (other.gameObject.GetComponent<Workers>().targetedBuilding == gameObject.GetInstanceID())
        {
            builders.Add(other.gameObject);
            other.gameObject.GetComponent<Workers>().agent.isStopped = true;
        }

        if (hitPoints <= maxHitPoints && other.gameObject.GetComponent<Workers>().targetedBuilding == gameObject.GetInstanceID() && hasFinishedBuilding)
        {
            Debug.Log(builders.Count);
            builders.Add(other.gameObject);
            StartCoroutine(RepairBuilding());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canPlace = true;
        if (!other.CompareTag("Worker")) return;
        builders.Remove(other.gameObject);
    }

    public override void OnHit(int damage, Entity instigator)
    {
        base.OnHit(damage,instigator);
        if (isEnemy) return;
        buildingCollider.isTrigger = true;
    }

    public virtual void OnBuildingComplete()
    {
        if (hasFinishedBuilding)
        {
            PlayerManager.Instance.CheckIfPowerIsSufficient(costOfPower, false);
        }
    }

    public void OnNoPower()
    {
        OnDeselect();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        if (buildingHitBox == null)
        {
            buildingHitBox = GetComponent<NavMeshObstacle>();
        }
        canPlace = true;
        isDead = false;
        if (isEnemy)
        {
            buildingHitBox.enabled = true;
            buildingHitBox.carving = true;
        }
        else
        {
            buildingHitBox.enabled = false;
            buildingHitBox.carving = false;
        }
    }
}
