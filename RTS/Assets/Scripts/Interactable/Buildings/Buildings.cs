using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class Buildings : Entities
{
    //BUILDING BOOL;
    [SerializeField] private int amountOfHpPerSecond;
    [SerializeField] protected bool canProduceUnits;
    protected bool canPlace = true;
    protected bool hasFinishedBuilding = false;

    private float buildPercentage;
    
    [SerializeField] private Vector3 dropBuildingIntoFloor;

    [SerializeField] protected GameObject floatingText;

    private Vector3 targetToMoveBuilding = new Vector3(0, 0.25f,0);

    private MeshRenderer[] buildingRenderer;

    [SerializeField] protected List<GameObject> buildableUnits = new List<GameObject>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        buildingRenderer = GetComponentsInChildren<MeshRenderer>(true);
        if (canProduceUnits)
        {
            buildableUnits = UnitManager.Instance.buildableUnits;
        }
        Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        _selectionBox.transform.localScale = transform.localScale * 3;
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
            StartCoroutine(BuildBuilding());
            
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
        targetToMoveBuilding = new Vector3(transform.position.x,0.25f,transform.position.z);
        var positionToSpawnTextObject = new Vector3(transform.position.x, 3, transform.position.z);
        var textObject = Instantiate(floatingText, positionToSpawnTextObject, Quaternion.Euler(90,0,0), transform);

        var target = new Vector3(transform.position.x,0.5f,transform.position.z);
        while (transform.position != target && hitPoints <= maxHitPoints)
        {
            if (isDead)
            {
                StopCoroutine(BuildBuilding());
                Destroy(gameObject);
            }
            Mathf.Clamp(hitPoints, 0, maxHitPoints);
            hitPoints += amountOfHpPerSecond;
            var equation = (hitPoints/maxHitPoints) * 100;
            textObject.GetComponent<TextMeshPro>().text = "Building: " + equation.ToString("F0")+"%";
            transform.position = Vector3.MoveTowards(transform.position, targetToMoveBuilding, step);
            yield return new WaitForSeconds(0.1f);
        }
        hasFinishedBuilding = true;
        Destroy(textObject);
        yield return new WaitForSeconds(0.1f);
    }
    private void CancelBuilding(bool hasClicked)
    {
        if (!hasClicked) return;
        UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        PlayerManager.Instance.hasBuildingInHand = false;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground")) return;
        canPlace = false;
    }

    private void OnTriggerExit(Collider other)
    {
        canPlace = true;
    }
}
