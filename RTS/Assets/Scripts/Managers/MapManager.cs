using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Managers
{


    public class MapManager : MonoBehaviour
    {
        private static MapManager _instance;

        public static MapManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MapManager>();
                }

                return _instance;
            }
        }

        [SerializeField] private List<GameObject> mapsToChooseFrom = new List<GameObject>();
        private SupplyDepo[] _supplyDepos;

        public GameObject currentlyActiveMap;

        // Start is called before the first frame update
        void Awake()
        {
            SelectMap(1);
            _supplyDepos = FindObjectsOfType<SupplyDepo>();
        }


        private void SelectMap(int mapSelected)
        {
            if (currentlyActiveMap != null)
            {
                currentlyActiveMap.SetActive(false);
            }

            var mapChosen = mapsToChooseFrom[mapSelected];
            mapChosen.SetActive(true);
            currentlyActiveMap = mapChosen;
        }

        public Vector2 ReturnSizeOfMap()
        {
            var colliders = currentlyActiveMap.GetComponent<Collider>();
            Vector2 mapSize = Vector2.zero;

            var bounds = colliders.bounds;
            mapSize.x = bounds.size.x / 2;
            mapSize.y = bounds.size.z / 2;

            return mapSize;
        }
        public SupplyDepo[] GetAllSupplyDepos()
        {
            return _supplyDepos;
        }
    }

}
