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
public class Buildings : Entity
{
    private readonly List<GameObject> builders = new List<GameObject>();
    
    //BUILDING BOOL;
    protected bool canPlace = true;
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

        buildingRenderer = GetComponentsInChildren<MeshRenderer>();
        

        Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        selectionBox.transform.localScale = transform.localScale * 3;
        isBuilding = true;
        //References the obstacle collider for other agents to avoid
        buildingHitBox = GetComponent<NavMeshObstacle>();
        buildingHitBox.enabled = false;
        buildingHitBox.carving = false;
        
        buildingCollider = GetComponentInChildren<BoxCollider>();
        isSelectable = true;
    }


    public override void Subscribe(CharacterInput publisher)
    {
        publisher.hasClicked += PlaceBuilding;
        publisher.mousePosition += CanPlaceBuilding;
        publisher.hasLeftClickedMouse += CancelBuilding;
    }

    public override void UnSubscribe(CharacterInput publisher)
    {
        publisher.mousePosition -= CanPlaceBuilding;
        publisher.hasLeftClickedMouse -= CancelBuilding;
        publisher.hasClicked -= PlaceBuilding;
    }

    public override void OnClicked()
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
            
            Debug.Log("This is a building");
        }
        else
        {
            HUD.SetCursor(CursorStates.Select);
            UnitManager.Instance.selectedNonLethalUnits.Remove(gameObject);
            PlayerManager.Instance.hasSelectedNonLethalUnits = false;
        }
        BuildingManager.Instance.currentSelectedBuilding = gameObject;
        UIManager.Instance.ShowPanels(true,3);
        base.OnClicked();
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        UIManager.Instance.ShowPanels(false,3);
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

    protected virtual IEnumerator BuildBuilding()
    {
        PlayerManager.Instance.MoneyPlayerHad = PlayerManager.Instance.AmountOfMoneyPlayerHas;
        UIManager.Instance.DecreasePlayerMoney();

        float step = buildingSpeed * Time.deltaTime;
        targetToMoveBuilding = new Vector3(transform.position.x, Mathf.Abs(dropBuildingIntoFloor.y), transform.position.z);
        var positionToSpawnTextObject = new Vector3(transform.position.x, 3f, transform.position.z);
        textObject = Instantiate(floatingText, positionToSpawnTextObject, Quaternion.Euler(90, 0, 0));
        PlayerManager.Instance.playerMoneyRemoval = PlayerManager.Instance.StartCoroutine(PlayerManager.Instance.RemoveMoney(this));


        var target = new Vector3(transform.position.x, targetToMoveBuilding.y, transform.position.z);
        while (transform.position != target && hitPoints < maxHitPoints)
        {
            if (isDead)
            {
                StopCoroutine(BuildBuilding());
                Destroy(gameObject);
            }

            while (builders.Count == 0) {
                yield return new WaitForSeconds (0.2f);
            }

            while (PlayerManager.Instance.AmountOfMoneyPlayerHas <= 0)
            {
                yield return new WaitForSeconds(0.2f);
            }

            hitPoints = Mathf.Clamp(hitPoints, 0, maxHitPoints);
            if (hitPoints < maxHitPoints)
            {
                hitPoints += amountOfHpPerSecond;
                var equation = (hitPoints / maxHitPoints) * 100;
                textObject.GetComponent<TextMeshPro>().text = "Building: " + equation.ToString("F0") + "%";
                Mathf.Clamp(equation, 0, 100);
                if (transform.position == target)
                {
                    hitPoints = maxHitPoints;
                    equation = 100;
                }
                else if (hitPoints >= maxHitPoints)
                {
                    transform.position = target;
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, targetToMoveBuilding, step);
            yield return new WaitForSeconds(0.1f);
        }
        hitPoints = maxHitPoints;
        buildingCollider.isTrigger = false;
        buildingHitBox.enabled = true;
        buildingHitBox.carving = true;
        hasFinishedBuilding = true;
        PlayerManager.Instance.playerMoneyRemoval  = null;
        PlayerManager.Instance.CheckIfPowerIsSufficient(costOfPower);
        UIManager.Instance.UpdateRequiredPowerText();
        //Destroys the text box.
        Destroy(textObject);

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

        if (gameObject.GetComponent<PowerReactor>())
        {
            gameObject.GetComponent<PowerReactor>().AddPowerToPlayer();
        }
        else if (gameObject.GetComponent<Turret>())
        {
            gameObject.GetComponent<Turret>().ActivateTurret();
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
                Destroy(gameObject);
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
            Destroy(gameObject);
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

    public override void OnHit(int damage)
    {
        base.OnHit(damage);
        buildingCollider.isTrigger = true;
    }

    protected virtual void OnDestroy()
    {
        if (textObject != null)
        {
            Destroy(textObject);
        }
        OnDeselect();
    }
    
}
