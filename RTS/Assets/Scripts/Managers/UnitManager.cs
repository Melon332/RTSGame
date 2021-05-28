using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    
    public class UnitManager : MonoBehaviour
    {
        private static UnitManager _instance;

        public static UnitManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UnitManager>();
                }

                return _instance;

            }
        }

        public List<GameObject> buildableUnits = new List<GameObject>();
    }
}

