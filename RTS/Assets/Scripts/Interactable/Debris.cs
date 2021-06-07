using UnityEngine;
using Player;
using Managers;

namespace Interactable
{
    public class Debris : Entities
    {
        protected override void Start()
        {
            selectionBox = GetComponentInChildren<SelectionBox>().gameObject;
            if (selectionBox == null) return;
            selectionBox.SetActive(false);
            selectionBox.transform.localScale = gameObject.transform.localScale * 2;
            canBeAttacked = true;
        }

        public override void OnClicked()
        {
            if (PlayerManager.Instance.hasSelectedUnits) return;
            base.OnClicked();
            Debug.Log("I am a debris look at me!");
        }

        public override void OnHit(int damage)
        {
            base.OnHit(damage);
            UnitManager.SelectableUnits.Remove(gameObject);
            UnitManager.Instance.selectedAttackingUnits.Remove(gameObject);
        }
    }
}
