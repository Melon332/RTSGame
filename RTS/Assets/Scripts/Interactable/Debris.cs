using UnityEngine;
using Player;
using Managers;
using UnityEngine.AI;

namespace Interactable
{
    [RequireComponent(typeof(NavMeshObstacle))]
    public class Debris : Entity
    {
        protected override void Start()
        {
            canBeAttacked = true;
        }

        public override void OnClicked()
        {
            if (PlayerManager.Instance.hasSelectedUnits) return;
            base.OnClicked();
            Debug.Log("I am a debris look at me!");
        }

        public override void OnHit(int damage, Entity instigator)
        {
            base.OnHit(damage,instigator);
            UnitManager.SelectableUnits.Remove(gameObject);
            UnitManager.Instance.selectedAttackingUnits.Remove(gameObject);
        }
    }
}
