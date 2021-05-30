using UnityEngine;

namespace Interactable
{
    public class Debris : Entities, IDestructable
    {
        protected override void Start()
        {
            canBeAttacked = true;
        }
    }
}
