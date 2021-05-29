using UnityEngine;

namespace Player
{

[RequireComponent(typeof(PlayerInputMouse))]
    public class PlayerHandler : MonoBehaviour
    {


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
        [HideInInspector] public CameraController cameraController;

        [HideInInspector] public CharacterInput characterInput;
        private PlayerSelectedUnits _playerSelectedUnits;

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
