using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            //Checks if the player has a building in hand.
            if (PlayerManager.Instance.hasBuildingInHand) return;
            if (!hasClicked) return;
            if (!PlayerInputMouse.IsPointerOverUIObject())
            {
                //Shoots a ray from the mouse position
                _ray = _rtsCamera.ScreenPointToRay(Input.mousePosition);
                if (PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out var hit))
                {
                    if(hit.collider.GetComponent<Entity>())
                    {
                        if (!PlayerInputMouse.IsMouseOverEnemy())
                        {
                            //Checks if that ray hit something interactable
                            if (hit.collider.GetComponent<IInteractable>() != null)
                            {
                                //CLEARING LIST AND RESETTING OBJECTS
                                PlayerManager.Instance.hasSelectedNonLethalUnits = false;
                                PlayerManager.Instance.hasSelectedUnits = false;
                                foreach (var units in UnitManager.Instance.selectedAttackingUnits)
                                {
                                    units.GetComponent<IInteractable>().OnDeselect();
                                }

                                foreach (var workers in UnitManager.Instance.selectedNonLethalUnits)
                                {
                                    if (workers != null)
                                    {
                                        workers.GetComponent<IInteractable>().OnDeselect();
                                    }
                                }

                                UnitManager.Instance.selectedAttackingUnits.Clear();
                                UnitManager.Instance.selectedNonLethalUnits.Clear();
                            }
                        }


                        hit.collider.GetComponent<IInteractable>().OnClicked();
                        //Calls a OnClicked Method to the clicked unit
                        //Adds it to a list but if it already exists on the list, continue
                        if (!UnitManager.Instance.selectedAttackingUnits.Contains(hit.collider.gameObject))
                        {
                            //If it's a unit that you can attack with, add it to the lethal units list.
                            if (hit.collider.CompareTag("Units"))
                            {
                                PlayerManager.Instance.hasSelectedUnits = true;
                                UnitManager.Instance.selectedAttackingUnits.Add(hit.collider.gameObject);
                            }
                            else if (hit.collider.GetComponent<Entity>().isSelectable && !hit.collider.GetComponent<Entity>().canAttack && !hit.collider.GetComponent<Entity>().isBuilding)
                            {
                                PlayerManager.Instance.hasSelectedNonLethalUnits = true;
                            }
                            else if(hit.collider.GetComponent<Entity>().isBuilding)
                            {
                                PlayerManager.Instance.hasSelectedBuilding = true;
                            }
                            if (!UnitManager.Instance.selectedNonLethalUnits.Contains(hit.collider.gameObject))
                            {
                                if (!UnitManager.Instance.selectedAttackingUnits.Contains(hit.collider.gameObject))
                                {
                                    UnitManager.Instance.selectedNonLethalUnits.Add(hit.collider.gameObject);
                                }
                            }
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
            //Selection box size
            Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
            Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);


            foreach (var unit in UnitManager.SelectableUnits)
            {
                //Checks which units are in the selection box, if it's empty, return.
                if (unit == null) return;
                Vector3 screenPos = _rtsCamera.WorldToScreenPoint(unit.transform.position);
                //checks if the selection box is in world space
                if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                {
                    //If the selection box already has that unit in the list, continue searching.
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
            //Resets the position of the box and tells the game that the player isn't holding down button
            _startPos = Vector2.zero;
            holdingDownButton = false;
        }

        private void SelectAllUnitsOfSort(bool hasClicked, bool hasShiftClicked)
        {
            if (!hasClicked || !hasShiftClicked) return;
            if (!PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out var hit)) return;
            if (hit.collider.GetComponent<IInteractable>() == null) return;
            foreach (var units in UnitManager.SelectableUnits)
            {
                //Checks if the unit is in any list, if it isn't continue
                if (units.GetComponent<Entity>().GetType() != hit.collider.GetComponent<Entity>().GetType() ||
                    !units.GetComponent<Entity>().hasBeenConstructed) continue;
                if (units.GetComponent<Entity>().canAttack)
                {
                    if (UnitManager.Instance.selectedAttackingUnits.Contains(units.gameObject)) continue;
                    PlayerManager.Instance.hasSelectedUnits = true;
                    UnitManager.Instance.selectedAttackingUnits.Add(units.gameObject);
                    units.GetComponent<IInteractable>().OnClicked();
                }
                else
                {
                    if (UnitManager.Instance.selectedNonLethalUnits.Contains(hit.collider.gameObject)) continue;
                    PlayerManager.Instance.hasSelectedNonLethalUnits = true;
                    UnitManager.Instance.selectedNonLethalUnits.Add(units.gameObject);
                    units.GetComponent<IInteractable>().OnClicked();
                }
            }
        }

        private void DeSelectUnits(bool hasLeftClicked)
        {
            if (!hasLeftClicked) return;
            PlayerManager.Instance.hasSelectedNonLethalUnits = false;
            PlayerManager.Instance.hasSelectedUnits  = false;
            foreach (var units in UnitManager.Instance.selectedAttackingUnits)
            {
                units.GetComponent<IInteractable>().OnDeselect();
            }
            foreach (var workers in UnitManager.Instance.selectedNonLethalUnits)
            {
                if (workers != null)
                {
                    workers.GetComponent<IInteractable>().OnDeselect();
                }
            }
            UnitManager.Instance.selectedAttackingUnits.Clear();
            UnitManager.Instance.selectedNonLethalUnits.Clear();
        }

        public void Subscribe(CharacterInput publisher)
        {
            publisher.hasClickedAndShiftClicked += SelectAllUnitsOfSort;
            publisher.hasClicked += ClickedOnUnit;
            publisher.hasHeldDownButton += SelectingMultipleUnits;
            publisher.hasReleasedButton += ReleaseSelectionBox;
            publisher.hasLeftClickedMouse += DeSelectUnits;
        }

        public void UnSubscribe(CharacterInput publisher)
        {
            publisher.hasClickedAndShiftClicked -= SelectAllUnitsOfSort;
            publisher.hasClicked -= ClickedOnUnit;
            publisher.hasHeldDownButton -= SelectingMultipleUnits;
            publisher.hasReleasedButton -= ReleaseSelectionBox;
            publisher.hasLeftClickedMouse -= DeSelectUnits;
        }
    }
}
