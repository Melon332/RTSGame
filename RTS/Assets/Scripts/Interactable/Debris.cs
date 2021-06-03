using UnityEngine;
using Player;

namespace Interactable
{
    public class Debris : Entities
    {
        protected override void Start()
        {
            _selectionBox = GetComponentInChildren<SelectionBox>().gameObject;
            if (_selectionBox == null) return;
            _selectionBox.SetActive(false);
            _selectionBox.transform.localScale = gameObject.transform.localScale * 2;
            canBeAttacked = true;
        }

        public override void OnClicked()
        {
            if (PlayerManager.Instance.hasSelectedUnits) return;
            base.OnClicked();
            Debug.Log("I am a debris look at me!");
        }
    }
}
