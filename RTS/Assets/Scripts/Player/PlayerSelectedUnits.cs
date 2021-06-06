using System;
using System.Collections.Generic;
using Enums;
using Interactable;
using Managers;
using UnityEngine;

namespace Player
{
    public class PlayerSelectedUnits : MonoBehaviour, ISubscriber
    {
        [SerializeField] private RectTransform selectionBox;
        private Ray _ray;
        private Camera _rtsCamera;

        private Vector2 _startPos;
        
        public static bool holdingDownButton;


        // Start is called before the first frame update
        void Start()
        {
            _rtsCamera = GetComponent<Camera>();
        }

        private void ClickedOnUnit(bool hasClicked)
        {
            if (PlayerManager.Instance.hasBuildingInHand) return;
            if (!hasClicked) return;
            _ray = _rtsCamera.ScreenPointToRay(Input.mousePosition);
            if (PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out var hit))
            {
                if (hit.collider.GetComponent<IInteractable>() != null)
                {
                    hit.collider.GetComponent<IInteractable>().OnClicked();
                    if (!UnitManager.Instance.selectedAttackingUnits.Contains(hit.collider.gameObject))
                    {
                        if (hit.collider.CompareTag("Units"))
                        {
                            PlayerManager.Instance.hasSelectedUnits = true;
                            UnitManager.Instance.selectedAttackingUnits.Add(hit.collider.gameObject);
                        }
                        else if (hit.collider.CompareTag("Buildings") || hit.collider.CompareTag("EnemyUnits") || hit.collider.CompareTag("Debris"))
                        {
                            UnitManager.Instance.selectedNonLethalUnits.Add(hit.collider.gameObject);
                        }
                        else
                        {
                            UnitManager.Instance.selectedNonLethalUnits.Add(hit.collider.gameObject);
                            PlayerManager.Instance.hasSelectedNonLethalUnits = true;
                        }
                    }
                }
            }

            if (!PlayerInputMouse.IsPointerOverUIObject())
            {
                _startPos = Input.mousePosition;
            }
        }

        private void SelectingMultipleUnits(bool hasBeenHeldDown, Vector2 currMousePos)
        {
            if (!hasBeenHeldDown) return;
            if (_startPos != Vector2.zero)
            {
                holdingDownButton = true;

                var width = currMousePos.x - _startPos.x;
                var height = currMousePos.y - _startPos.y;
                
                if (!selectionBox.gameObject.activeInHierarchy)
                    selectionBox.gameObject.SetActive(true);

                selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
                selectionBox.anchoredPosition = _startPos + new Vector2(width / 2, height / 2);
            }
        }

        private void ReleaseSelectionBox(bool hasReleaseButton)
        {
            if (!hasReleaseButton) return;
            selectionBox.gameObject.SetActive(false);

            Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
            Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);


            foreach (var unit in UnitManager.SelectableUnits)
            {
                if (unit == null) return;
                Vector3 screenPos = _rtsCamera.WorldToScreenPoint(unit.transform.position);
                if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                {
                    if (UnitManager.Instance.selectedAttackingUnits.Contains(unit.gameObject)) continue;
                    if (unit.CompareTag("Units"))
                    {
                        PlayerManager.Instance.hasSelectedUnits = true;
                        UnitManager.Instance.selectedAttackingUnits.Add(unit.gameObject);
                    }
                    else
                    {
                        UnitManager.Instance.selectedNonLethalUnits.Add(unit.gameObject);
                        PlayerManager.Instance.hasSelectedNonLethalUnits = true;
                    }
                    unit.GetComponent<IInteractable>().OnClicked();
                }
            }
            _startPos = Vector2.zero;
            holdingDownButton = false;
        }

        private void DeSelectUnits(bool hasLeftClicked)
        {
            if (!hasLeftClicked) return;
            foreach (var units in UnitManager.Instance.selectedAttackingUnits)
            {
                units.GetComponent<IInteractable>().OnDeselect();
                PlayerManager.Instance.hasSelectedUnits  = false;
            }
            foreach (var workers in UnitManager.Instance.selectedNonLethalUnits)
            {
                if (workers != null)
                {
                    workers.GetComponent<IInteractable>().OnDeselect();
                    PlayerManager.Instance.hasSelectedNonLethalUnits = false;
                }
            }
            UnitManager.Instance.selectedAttackingUnits.Clear();
            UnitManager.Instance.selectedNonLethalUnits.Clear();
        }

        public void Subscribe(CharacterInput publisher)
        {
            publisher.hasClicked += ClickedOnUnit;
            publisher.hasHeldDownButton += SelectingMultipleUnits;
            publisher.hasReleasedButton += ReleaseSelectionBox;
            publisher.hasLeftClickedMouse += DeSelectUnits;
        }

        public void UnSubscribe(CharacterInput publisher)
        {
            publisher.hasClicked -= ClickedOnUnit;
            publisher.hasHeldDownButton -= SelectingMultipleUnits;
            publisher.hasReleasedButton -= ReleaseSelectionBox;
            publisher.hasLeftClickedMouse -= DeSelectUnits;
        }
    }
}
