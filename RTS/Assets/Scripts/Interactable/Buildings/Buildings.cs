using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Player;
using UnityEngine;

public class Buildings : Entities
{
    [SerializeField] protected bool canProduceUnits;
    protected bool canPlace = true;
    protected bool hasFinishedBuilding = false;
    public static bool hasBuildingInHand = false;

    private MeshRenderer[] buildingRenderer;

    [SerializeField] protected List<GameObject> buildableUnits = new List<GameObject>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        buildingRenderer = GetComponentsInChildren<MeshRenderer>();
        if (canProduceUnits)
        {
            buildableUnits = UnitManager.Instance.buildableUnits;
        }
        Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        _selectionBox.transform.localScale = transform.localScale * 2;
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
        base.OnClicked();
        Debug.Log("This is a building");
    }

    private void CanPlaceBuilding(RaycastHit mousePos)
    {
        if (hasFinishedBuilding) return;
        transform.position = new Vector3(mousePos.point.x, 0, mousePos.point.z);
        hasBuildingInHand = true;
        if (canPlace)
        {
            foreach (var buildingBlocks in buildingRenderer)
            {
                Debug.Log(!buildingBlocks.GetComponent<SelectionBox>());
                if (!buildingBlocks.GetComponent<SelectionBox>())
                {
                    buildingBlocks.material = BuildingManager.Instance.canPlaceBuilding;
                }
            }
        }
        else
        {
            foreach (var buildingBlocks in buildingRenderer)
            {
                if (!buildingBlocks.GetComponent<SelectionBox>())
                {
                    buildingBlocks.material = BuildingManager.Instance.cantPlaceBuilding;
                }
            }
        }
    }
    private void PlaceBuilding(bool place)
    {
        if (place && canPlace)
        {
            Debug.Log("Test");
            hasFinishedBuilding = true;
            hasBuildingInHand = false;
            UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        }
        else
        {
            Debug.Log("Sorry sir, I cant build there");
        } 
    }

    private void CancelBuilding(bool hasClicked)
    {
        if (!hasClicked) return;
        UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        hasBuildingInHand = false;
        Destroy(gameObject,0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground")) return;
        canPlace = false;
        Debug.Log(canPlace);
    }

    private void OnTriggerExit(Collider other)
    {
        canPlace = true;
    }
}
