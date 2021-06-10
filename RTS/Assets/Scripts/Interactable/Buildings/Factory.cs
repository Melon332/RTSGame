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
    [HideInInspector] public GameObject currentUnitConstructing;
    public List<GameObject> unitQueue = new List<GameObject>(9);

    private bool isConstructingUnit;
    
    private Coroutine createUnitCoroutine;
    
    private Coroutine ConstructUnit;
    private GameObject textObject;

    private Vector3 rallyPointPosition;

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

    protected virtual IEnumerator CreateUnit(GameObject textBox)
    {
        while (unitQueue.Any())
        {
            if (!isConstructingUnit)
            {
                //1. 
                yield return new WaitForSeconds(0.5f);
               currentUnitConstructing = Instantiate(unitQueue[0], transform.position, Quaternion.identity);
               currentUnitConstructing.GetComponent<MeshRenderer>().enabled = false;
               isConstructingUnit = true;
               textBox.SetActive(true);
            }
            else
            {
                var unitComponent = currentUnitConstructing.GetComponent<Units>();
                while(unitComponent.hitPoints < unitComponent.maxHitPoints)
                {
                    //Checks hit points and does an equation to convert it into % and set it into a text box
                    unitComponent.hitPoints = Mathf.Clamp(unitComponent.hitPoints, 0, unitComponent.maxHitPoints); 
                    unitComponent.hitPoints += unitComponent.amountOfHpPerSecond;
                    var equation = (unitComponent.hitPoints / unitComponent.maxHitPoints) * 100;
                    textBox.GetComponent<TextMeshPro>().text = "Constructing: " + equation.ToString("F0") + "%";
                    yield return new WaitForSeconds(0.2f);
                }
                isConstructingUnit = false;
                if (rallyPointPosition != Vector3.zero)
                {
                    unitComponent.MoveForward(rallyPointPosition);
                }
                else
                {
                    var position = new Vector3(unitComponent.transform.position.x, unitComponent.transform.position.y,
                        5f); 
                    unitComponent.MoveForward(position);
                }
                //1. Makes the unit interactable
                //2. Sets the current unit to null
                //3. Unit queue index 0 removed
                //4. Removed text box
                //5. Update unit count
                unitComponent.hasBeenConstructed = true;
                unitComponent.ActivateAllMesh();
                currentUnitConstructing = null;
                unitQueue.RemoveAt(0);
                textBox.SetActive(false);
                UIManager.Instance.UpdateUnitCount();
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
                yield return new WaitForSeconds(0.1f);
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
            UIManager.Instance.ShowPanels(true, 1);
            Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
    }

    public override void OnDeselect()
    {

        base.OnDeselect();
        UIManager.Instance.ShowPanels(false,1);
        UnSubscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
        BuildingManager.Instance.wantsToSetRallyPoint = false;
        
    }

    public override void Subscribe(CharacterInput publisher)
    {
        base.Subscribe(publisher);
        publisher.hasClicked += SetRallyPoint;
    }

    public override void UnSubscribe(CharacterInput publisher)
    {
        base.UnSubscribe(publisher);
        publisher.hasClicked -= SetRallyPoint;
    }
}
