using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;
using Enums;
using TMPro;

namespace Managers
{

    public class UIManager : MonoBehaviour
    {
        
        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManager>();
                }

                return _instance;
            }
        }
        
        //MAIN PANEL
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private Image imageOfUnit;
        
        [SerializeField] private GameObject mouseOrKeyboardText;
        //CURSORS VARIABLES
        public Texture2D[] cursors;
        private static Texture2D[] _cursorsStatic;
        
        [SerializeField] private GameObject buildingPanel;
        [SerializeField] private Button pullPanelDownButton;

        private bool panelIsDown;
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
            Cursor.SetCursor(_cursorsStatic[currentlySelectedState],Vector2.zero, CursorMode.ForceSoftware);
            if (currentlySelectedState == 8)
            {
                Cursor.SetCursor(null,Vector2.zero, CursorMode.ForceSoftware);
            }
        }

        public void BuildFactoryBuilding(int buildingIndex)
        {
            if(PlayerManager.Instance.hasBuildingInHand) return;
            BuildingManager.Instance.CreateBuilding(buildingIndex);
        }

        public void PullPanelDown()
        {
            var rectTransform = mainPanel.GetComponent<RectTransform>();
            if (!panelIsDown)
            {
                rectTransform.anchoredPosition = new Vector2(0, -299);
                panelIsDown = true;
                pullPanelDownButton.GetComponentInChildren<TextMeshProUGUI>().text = "Panel Up";
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(0, 0);
                panelIsDown = false;
                pullPanelDownButton.GetComponentInChildren<TextMeshProUGUI>().text = "Panel Down";
            }
        }

        public void ShowBuildingsPanel(bool hasSelectedWorker)
        {
            buildingPanel.SetActive(hasSelectedWorker);
        }

        public void PictureOfSelectedUnits(Sprite image)
        {
            imageOfUnit.sprite = image;
        }
    }
}
