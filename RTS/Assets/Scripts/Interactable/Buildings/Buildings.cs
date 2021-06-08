using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshObstacle))]
public class Buildings : Entities
{
    private List<GameObject> builders = new List<GameObject>();
    
    [SerializeField] private float amountOfHpPerSecond;
    //BUILDING BOOL;
    protected bool canPlace = true;
    protected bool hasFinishedBuilding = false;
    [HideInInspector] public bool hasPlacedBuilding;

    [SerializeField] private Vector3 dropBuildingIntoFloor;

    [SerializeField] protected GameObject floatingText;

    private Vector3 targetToMoveBuilding = new Vector3(0, -0.5f, 0);

    private MeshRenderer[] buildingRenderer;
    private NavMeshObstacle buildingHitBox;
    private BoxCollider buildingCollider;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        buildingRenderer = GetComponentsInChildren<MeshRenderer>(true);

        Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        selectionBox.transform.localScale = transform.localScale * 3;
        isBuilding = true;
        //References the obstacle collider for other agents to avoid
        buildingHitBox = GetComponent<NavMeshObstacle>();
        buildingHitBox.enabled = false;
        buildingHitBox.carving = false;
        
        buildingCollider = GetComponent<BoxCollider>();
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
        if (!hasFinishedBuilding) return;
        //Checks if the player has units selected.
        if (PlayerManager.Instance.hasSelectedUnits || PlayerManager.Instance.hasSelectedNonLethalUnits)
        {
            //Resets the units selection list so that the player can focus on the building
            foreach (var lethalUnits in UnitManager.Instance.selectedAttackingUnits)
            {
                lethalUnits.GetComponent<Entities>().OnDeselect();
            }
            foreach (var nonLethalUnit in UnitManager.Instance.selectedNonLethalUnits)
            {
                nonLethalUnit.GetComponent<Entities>().OnDeselect();
                PlayerManager.Instance.hasSelectedNonLethalUnits = false;
            }
            //Clears the list and tells the player it has no units selected.
            PlayerManager.Instance.hasSelectedUnits = false;
            PlayerManager.Instance.hasSelectedNonLethalUnits = false;
            UnitManager.Instance.selectedAttackingUnits.Clear();
            UnitManager.Instance.selectedNonLethalUnits.Clear();
        }
        HUD.SetCursor(CursorStates.Select);
        
        base.OnClicked();
        Debug.Log("This is a building");
        BuildingManager.Instance.currentSelectedBuilding = gameObject;
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
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
            buildingHitBox.enabled = true;
            buildingHitBox.carving = true;

            buildingCollider.transform.position = new Vector3(transform.position.x,
            targetToMoveBuilding.y, transform.position.z);

            //Drop the building into to floor to rebuild it.
            var position = transform.position;
            dropBuildingIntoFloor.x = position.x;
            dropBuildingIntoFloor.z = position.z;
            position = dropBuildingIntoFloor;
            transform.position = position;
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

    IEnumerator BuildBuilding()
    {
        var speed = 5f;

        float step = speed * Time.deltaTime;
        targetToMoveBuilding = new Vector3(transform.position.x, -dropBuildingIntoFloor.y, transform.position.z);
        var positionToSpawnTextObject = new Vector3(transform.position.x, 3, transform.position.z);
        var textObject = Instantiate(floatingText, positionToSpawnTextObject, Quaternion.Euler(90, 0, 0),
            transform);

        var target = new Vector3(transform.position.x, 0.5f, transform.position.z);
        while (transform.position != target && hitPoints <= maxHitPoints)
        {
            if (isDead)
            {
                StopCoroutine(BuildBuilding());
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
                textObject.GetComponent<TextMeshPro>().text = "Building: " + equation.ToString("F0") + "%";
            }
            transform.position = Vector3.MoveTowards(transform.position, targetToMoveBuilding, step);
            yield return new WaitForSeconds(0.1f);
        }

        hasFinishedBuilding = true;

        //Moves the builder away from the building
        foreach (var builder in builders)
        {
            var worker = builder.GetComponent<Workers>();
            var transformPosition = builder.transform.position;
            var position = new Vector3(transformPosition.x, transformPosition.y, transformPosition.z);
            position.z += -5f;
            worker.MoveBackAfterCompletingBuilding(position);
            worker.ClearBuildingID();
        }

        //Destroys the text box.
        Destroy(textObject);
        yield return new WaitForSeconds(0.1f);
    }
    
    private void CancelBuilding(bool hasClicked)
    {
        if (!hasPlacedBuilding)
        {
            if (!hasClicked) return;
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
            PlayerManager.Instance.hasBuildingInHand = false;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground")) return;
        canPlace = false;
        if (!other.CompareTag("Worker")) return;
        if (other.gameObject.GetComponent<Workers>().targetedBuilding == gameObject.GetInstanceID())
        {
            builders.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canPlace = true;
        if (!other.CompareTag("Worker")) return;
        builders.Remove(other.gameObject);
    }
   
}
