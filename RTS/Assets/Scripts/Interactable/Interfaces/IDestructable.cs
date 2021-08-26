using UnityEngine;

namespace Interactable
{
    public interface IDestructable
    {
        void OnHit(int damage,Entity instigator);
    }
}
