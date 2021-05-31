using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Enums;

namespace Managers
{

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject mouseOrKeyboardText;
        public Texture2D[] cursors;
        private static Texture2D[] _cursorsStatic;
        void Awake()
        {
            if (_cursorsStatic == null)
            {
                _cursorsStatic = cursors;
            }
            Time.timeScale = 0;
            SetCursorState((int)CursorStates.Select);
        }

        // Update is called once per frame
        void Update()
        {
            if (!GameManager.Instance.hasChosenTypeOfCamera)
            {
                AddMouseOrKeyboardMoveset();
            }
        }

        private void AddMouseOrKeyboardMoveset()
        {

            if (Input.GetKeyDown(KeyCode.Y))
            {
                PlayerHandler.PlayerHandlerInstance.characterInput = GameManager.Instance.player.AddComponent<PlayerInputKeyboard>();
                PlayerHandler.PlayerHandlerInstance.cameraController.Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
                PlayerHandler.PlayerHandlerInstance.characterInput = GameManager.Instance.player.GetComponent<PlayerInputMouse>();
                GameManager.Instance.hasChosenTypeOfCamera = true;
                mouseOrKeyboardText.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                mouseOrKeyboardText.SetActive(false);
            }
            Time.timeScale = 1;
        }

        public static void SetCursorState(int currentlySelectedState)
        {
            Cursor.SetCursor(_cursorsStatic[currentlySelectedState],Vector2.zero, CursorMode.ForceSoftware);
        }
    }
}
