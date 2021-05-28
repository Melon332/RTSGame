using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [HideInInspector] public bool hasChosenTypeOfCamera;
        [HideInInspector] public bool GameIsPaused => Time.deltaTime == 0;


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
        }
    }
}
