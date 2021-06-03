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
   public bool hasBuildingInHand = false;
   
}
