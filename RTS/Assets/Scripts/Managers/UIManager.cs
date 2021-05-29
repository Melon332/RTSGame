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
        public static Texture2D[] cursorsStatic;

        // Start is called before the first frame update
        void Awake()
        {
            if (cursorsStatic == null)
            {
                cursorsStatic = cursors;
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
                PlayerHandler.PlayerHandlerInstance.cameraController?.Subscribe(PlayerHandler.PlayerHandlerInstance.characterInput);
                PlayerHandler.PlayerHandlerInstance.characterInput = GameManager.Instance.player.GetComponent<PlayerInputMouse>();
                GameManager.Instance.hasChosenTypeOfCamera = true;
                mouseOrKeyboardText.SetActive(false);
                Time.timeScale = 1;
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                mouseOrKeyboardText.SetActive(false);
                Time.timeScale = 1;
            }
        }

        public static void SetCursorState(int currentlySelectedState)
        {
            Cursor.SetCursor(cursorsStatic[currentlySelectedState],Vector2.zero, CursorMode.ForceSoftware);
        }
    }
}
