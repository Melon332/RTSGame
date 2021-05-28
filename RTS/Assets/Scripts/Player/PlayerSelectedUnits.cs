using System;
using System.Collections.Generic;
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
        public static List<GameObject> selectableUnits = new List<GameObject>();
        public static bool holdingDownButton;
        

        // Start is called before the first frame update
        void Start()
        {
            _rtsCamera = GetComponent<Camera>();
        }

        private void ClickedOnUnit(bool hasClicked)
        {
            if (hasClicked)
            {
                RaycastHit hit;
                _ray = _rtsCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out hit))
                {
                    if (hit.collider.CompareTag("Units"))
                    {
                        if (!selectedUnits.Contains(hit.collider.gameObject))
                        {
                            selectedUnits.Add(hit.collider.gameObject);
                            hit.collider.GetComponent<MeshRenderer>().material.color = Color.blue;
                            hit.collider.GetComponent<IInteractable>().OnClicked();
                        }

                        Debug.Log("You have: " + selectedUnits.Count + " units selected!");
                    }
                    
                }

                _startPos = Input.mousePosition;
            }
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
            if (hasReleaseButton)
            {
                selectionBox.gameObject.SetActive(false);

                Vector2 min = selectionBox.anchoredPosition-(selectionBox.sizeDelta / 2);
                Vector2 max = selectionBox.anchoredPosition+(selectionBox.sizeDelta / 2);

                foreach (var unit in selectableUnits)
                {
                    Vector3 screenPos = _rtsCamera.WorldToScreenPoint(unit.transform.position);
                    if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                    {
                        selectedUnits.Add(unit.gameObject);
                        unit.GetComponent<MeshRenderer>().material.color = Color.blue;
                        unit.GetComponent<IInteractable>().OnClicked();
                    }
                }
                holdingDownButton = false;
            }
        }

        private void DeSelectUnits(bool hasLeftClicked)
        {
            if (hasLeftClicked)
            {
                foreach (var units in selectedUnits)
                {
                    units.GetComponent<MeshRenderer>().material.color = Color.gray;
                    units.GetComponent<Units>().DeSelected();
                }
                selectedUnits.Clear();
            }
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
