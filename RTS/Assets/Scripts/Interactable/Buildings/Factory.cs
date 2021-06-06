using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interactable;
using Managers;
using UnityEngine;
using TMPro;

public class Factory : Buildings
{
    private GameObject currentUnitConstructing;
    public List<GameObject> unitQueue = new List<GameObject>();

    private bool isConstructingUnit;
    
    private Coroutine testingStuff;
    
    private Coroutine ConstructUnit;
    private GameObject textObject;

    protected override void Start()
    {
        base.Start();
    }

    public void StartConstructing(int _unitIndex)
    {
        var positionToSpawnTextObject = new Vector3(transform.position.x, 3, transform.position.z);
        if (textObject == null)
        {
            textObject = Instantiate(floatingText, positionToSpawnTextObject, Quaternion.Euler(90, 0, 0),
            transform);
        }

        if (testingStuff == null)
        {
            testingStuff = StartCoroutine(CreateUnit(textObject, _unitIndex));
            Debug.Log("I am being called");
        }
    }

    IEnumerator CreateUnit(GameObject textBox, int unitIndex)
    {
        while (unitQueue.Any())
        {
            if (!isConstructingUnit)
            {
                yield return new WaitForSeconds(0.5f);
               currentUnitConstructing = Instantiate(unitQueue[unitIndex], transform.position, Quaternion.identity);
               isConstructingUnit = true;
               textBox.SetActive(true);
            }
            else
            {
                var unitComponent = currentUnitConstructing.GetComponent<Units>();
                while(unitComponent.hitPoints <= unitComponent.maxHitPoints)
                {
                    unitComponent.hitPoints = Mathf.Clamp(unitComponent.hitPoints, 0, unitComponent.maxHitPoints); 
                    unitComponent.hitPoints += unitComponent.amountOfHpPerSecond;
                    var equation = (unitComponent.hitPoints / unitComponent.maxHitPoints) * 100;
                    textBox.GetComponent<TextMeshPro>().text = "Constructing: " + equation.ToString("F0") + "%";
                    yield return new WaitForSeconds(0.2f);
                }
                isConstructingUnit = false;
                var position = new Vector3(unitComponent.transform.position.x, unitComponent.transform.position.y,
                    5f);
                unitComponent.MoveForward(position);
                unitComponent.hasBeenConstructed = true;
                currentUnitConstructing = null;
                unitQueue.RemoveAt(0);

                if (!unitQueue.Any())
                {
                    StopCoroutine(testingStuff);
                    testingStuff = null;
                    textBox.SetActive(false);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public override void OnClicked()
    {
        base.OnClicked();
        UIManager.Instance.ShowUnitPanel(true);
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        UIManager.Instance.ShowUnitPanel(false);
    }
}
