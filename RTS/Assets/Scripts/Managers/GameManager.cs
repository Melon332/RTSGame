using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [HideInInspector] public bool hasChosenTypeOfCamera;

        [HideInInspector] public GameObject player;


        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                }

                return _instance;
            }
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            player = GameObject.Find("Main Camera");
            player.transform.position = new Vector3(MapManager.FindPlayerPosition().transform.position.x,
                10, MapManager.FindPlayerPosition().transform.position.z);
        }
    }
}
