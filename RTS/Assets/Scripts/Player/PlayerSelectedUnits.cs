using System;
using System.Collections.Generic;
using Enums;
using Interactable;
using UnityEngine;

namespace Player
{
    public class PlayerSelectedUnits : MonoBehaviour, ISubscriber
    {
        [SerializeField] private RectTransform selectionBox;
        private Ray _ray;
        private Camera _rtsCamera;

        private Vector2 _startPos;

        [SerializeField] private List<GameObject> selectedUnits = new List<GameObject>();
        public static readonly List<GameObject> SelectableUnits = new List<GameObject>();
        public static bool holdingDownButton;
        public static bool hasSelectedUnits = false;
        

        // Start is called before the first frame update
        void Start()
        {
            _rtsCamera = GetComponent<Camera>();
        }

        private void ClickedOnUnit(bool hasClicked)
        {
            if (!hasClicked) return;
            _ray = _rtsCamera.ScreenPointToRay(Input.mousePosition);
            if (PlayerHandler.PlayerHandlerInstance.cameraController.GetMousePosition(out var hit))
            {
                if (hit.collider.GetComponent<IInteractable>() != null)
                {
                    hit.collider.GetComponent<IInteractable>().OnClicked();
                    if (!selectedUnits.Contains(hit.collider.gameObject))
                    {
                        selectedUnits.Add(hit.collider.gameObject);
                        Debug.Log("You have: " + selectedUnits.Count + " units selected!");
                        hasSelectedUnits = true;
                        HUD.SetCursor(CursorStates.Move);
                    }
                }
            }

            _startPos = Input.mousePosition;
        }

        private void SelectingMultipleUnits(bool hasBeenHeldDown, Vector2 currMousePos)
        {
            if (hasBeenHeldDown)
            {
                holdingDownButton = true;
                if (!selectionBox.gameObject.activeInHierarchy)
                    selectionBox.gameObject.SetActive(true);

                float width = currMousePos.x - _startPos.x;
                float height = currMousePos.y - _startPos.y;

                selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
                selectionBox.anchoredPosition = _startPos + new Vector2(width / 2, height / 2);
            }
        }

        private void ReleaseSelectionBox(bool hasReleaseButton)
        {
            if (!hasReleaseButton) return;
            selectionBox.gameObject.SetActive(false);

            Vector2 min = selectionBox.anchoredPosition-(selectionBox.sizeDelta / 2);
            Vector2 max = selectionBox.anchoredPosition+(selectionBox.sizeDelta / 2);

            foreach (var unit in SelectableUnits)
            {
                Vector3 screenPos = _rtsCamera.WorldToScreenPoint(unit.transform.position);
                if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                {
                    if (selectedUnits.Contains(unit.gameObject)) continue;
                    selectedUnits.Add(unit.gameObject);
                    unit.GetComponent<IInteractable>().OnClicked();
                    hasSelectedUnits = true;
                    Debug.Log("You have: " + selectedUnits.Count + " units selected!");
                    HUD.SetCursor(CursorStates.Move);
                }
            }
            holdingDownButton = false;
        }

        private void DeSelectUnits(bool hasLeftClicked)
        {
            if (!hasLeftClicked) return;
            foreach (var units in selectedUnits)
            {
                units.GetComponent<IInteractable>().OnDeselect();
                units.GetComponent<Entities>()._selectionBox.SetActive(false);
                hasSelectedUnits = false;
            }
            selectedUnits.Clear();
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
        }
    }
}
