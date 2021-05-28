using UnityEngine;

namespace Player
{


    public class PlayerHandler : MonoBehaviour
    {
        [HideInInspector] public CameraController cameraController;

        public CharacterInput characterInput;
        private PlayerSelectedUnits _playerSelectedUnits;


        private static PlayerHandler _instance;

        public static PlayerHandler PlayerHandlerInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PlayerHandler>();
                }

                return _instance;
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            characterInput = GetComponent<CharacterInput>();
            cameraController = GetComponent<CameraController>();
            _playerSelectedUnits = GetComponent<PlayerSelectedUnits>();

            cameraController.Subscribe(characterInput);
            _playerSelectedUnits.Subscribe(characterInput);
        }

        private void OnDisable()
        {
            cameraController.UnSubscribe(characterInput);
            _playerSelectedUnits.UnSubscribe(characterInput);
        }
    }
}
