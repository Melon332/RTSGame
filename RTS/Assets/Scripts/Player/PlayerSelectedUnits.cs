using System.Collections.Generic;
using Interactable;
using UnityEngine;

namespace Player
{
    public class PlayerSelectedUnits : MonoBehaviour,ISubscriber
    {
        [SerializeField] private RectTransform selectionBox;
        private Ray _ray;
        private Camera _rtsCamera;

        private Vector2 _startPos;

        [SerializeField] private List<GameObject> selectedUnits = new List<GameObject>();
        // Start is called before the first frame update
        void Start()
        {
            _rtsCamera = GetComponent<Camera>();
        }
        private void ClickedOnSomething(bool hasClicked)
        {
            if (hasClicked)
            {
                RaycastHit hit;
                _ray = _rtsCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out hit))
                {
                    if (hit.collider.GetComponent<IInteractable>() != null)
                    {
                        if (!selectedUnits.Contains(hit.collider.gameObject))
                        {
                            selectedUnits.Add(hit.collider.gameObject);
                            hit.collider.GetComponent<MeshRenderer>().material.color = Color.blue;
                            hit.collider.GetComponent<IInteractable>().OnClicked();
                        }
                    }
                    
                    else
                    {
                        for (int i = 0; i < selectedUnits.Count; i++)
                        {
                            selectedUnits[i].GetComponent<MeshRenderer>().material.color = Color.black;
                        }
                        selectedUnits.Clear();
                    }
                }

                _startPos = Input.mousePosition;
            }
        }

        private void SelectingMultipleUnits(bool hasBeenHeldDown,Vector2 currMousePos)
        {
            if (hasBeenHeldDown)
            {
                if(!selectionBox.gameObject.activeInHierarchy)
                    selectionBox.gameObject.SetActive(true);

                float width = currMousePos.x - _startPos.x;
                float height = currMousePos.y - _startPos.y;

                selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
                selectionBox.anchoredPosition = _startPos + new Vector2(width / 2, height / 2);
            }
        }

        public void Subscribe(CharacterInput publisher)
        {
            publisher.hasClicked += ClickedOnSomething;
            publisher.hasHeldDownButton += SelectingMultipleUnits;
        }

        public void UnSubscribe(CharacterInput publisher)
        {
            publisher.hasClicked -= ClickedOnSomething;
        }
    }
}
