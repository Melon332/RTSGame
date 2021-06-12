﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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

        [SerializeField] private List<GameObject> panels = new List<GameObject>();
        [SerializeField] private Button pullPanelDownButton;
        [SerializeField] private TextMeshProUGUI playerMoney;
        [SerializeField] private TextMeshProUGUI playerUnit;
        [SerializeField] private TextMeshProUGUI playerPower;
        [SerializeField] private TextMeshProUGUI requiredPower;
        [SerializeField] private RawImage minimapHUD;

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
           panels[0].SetActive(hasSelectedWorker);
        }
        /// <summary>
        /// Specify if you want to show or hide panel and provide an panel number
        /// </summary>
        /// <param name="wantsToShowPanel"></param>
        /// <param name="panelNumber"></param>
        public void ShowPanels(bool wantsToShowPanel, int panelNumber)
        {
            panels[panelNumber].SetActive(wantsToShowPanel);
        }

        public void PictureOfSelectedUnits(Sprite image)
        {
            imageOfUnit.sprite = image;
        }

        public void DecreasePlayerMoney()
        {
            playerMoney.text = "Money: " + PlayerManager.Instance.AmountOfMoneyPlayerHas+ "$";
        }

        public IEnumerator IncreasePlayerMoney(int money)
        {
            while (money > 0)
            {
                PlayerManager.Instance.AmountOfMoneyPlayerHas++;
                money--;
                playerMoney.text = "Money: " + PlayerManager.Instance.AmountOfMoneyPlayerHas+ "$";
                yield return new WaitForSeconds(0.0001f);
            } 
        }

        public void PlayerPowerText()
        {
            playerPower.text = "Power: " + PlayerManager.Instance.AmountOfPowerPlayerHas;
        }

        public void UpdateRequiredPowerText()
        {
            requiredPower.text = "Required Power: " + PlayerManager.Instance.RequiredPower;
        }
        public void UpdatePlayerMoney()
        {
            playerMoney.text = "Money: " + PlayerManager.Instance.AmountOfMoneyPlayerHas+ "$";
        }

        public void UpdateUnitCount()
        {
            playerUnit.text = "Units created: " + UnitManager.SelectableUnits.Count;
        }

        public void MiniMapState(bool state)
        {
            minimapHUD.gameObject.SetActive(state);
        }
    }
}
