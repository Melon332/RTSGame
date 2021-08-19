using System;
using System.Collections;
using System.Collections.Generic;
using Interactable;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;

    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerManager>();
            }

            return _instance;
        }
    }


   [HideInInspector] public bool hasSelectedUnits = false;
   [HideInInspector] public bool hasSelectedNonLethalUnits = false;
   [HideInInspector] public bool hasSelectedBuilding = false;
   [HideInInspector] public bool hasBuildingInHand = false;
   [HideInInspector] public bool hasSelectedHarvester = false;
   [HideInInspector] public bool hasEnoughPower;
   public Coroutine playerMoneyRemoval;
   public int AmountOfMoneyPlayerHas { get; set; }
   public int MoneyPlayerHad { get; set; }
   public int AmountOfPowerPlayerHas { get; set; }
   public int RequiredPower {  get; private set; }
   public int startingMoney;

   private void Awake()
   {
       hasEnoughPower = true;
   }

   private void Start()
   {
       AmountOfPowerPlayerHas = 50;
       AmountOfMoneyPlayerHas += startingMoney;
       UIManager.Instance.UpdatePlayerMoney();
       UIManager.Instance.PlayerPowerText();
       UIManager.Instance.UpdateRequiredPowerText();
   }

   public void CheckIfPowerIsSufficient(int powerAmount, bool removePower)
   {
       if (!removePower)
       {
           RequiredPower += powerAmount;
       }
       else
       {
           RequiredPower -= powerAmount;
       }
       //Checks if power is sufficient
       if (RequiredPower < AmountOfPowerPlayerHas)
       {
           UIManager.Instance.MiniMapState(true);
           Debug.Log("Enough Power");
           Debug.Log(RequiredPower);
           hasEnoughPower = true;
       }
       else
       {
           Debug.Log("Insufficient power!");
           UIManager.Instance.MiniMapState(false);
           hasEnoughPower = false;
       }
   }
   public IEnumerator RemoveMoney(Entity obj)
   {

       var objectCost =+ obj.objectCost;
       Debug.Log(objectCost +" Starting value");
       while(objectCost > 0 && AmountOfMoneyPlayerHas > 0)
       {
           AmountOfMoneyPlayerHas -= 5;
           objectCost -= 5;
           UIManager.Instance.UpdatePlayerMoney();
           yield return new WaitForSeconds(0.00010f);
       }
        Debug.Log("Construction complete!");
   }

   public IEnumerator AddMoney(Entity obj)
   {
       var objectCost =+ obj.objectCost;
       Debug.Log(objectCost +" Starting value");
       while(objectCost > 0 && AmountOfMoneyPlayerHas > 0)
       {
           Debug.Log("hell");
           AmountOfMoneyPlayerHas += 5;
           objectCost -= 5;
           UIManager.Instance.UpdatePlayerMoney();
           yield return new WaitForSeconds(0.00010f);
       }
       Debug.Log("Building destroyed!");
   }

}
