using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
   public int AmountOfMoneyPlayerHas { get; set; }
   
}
