using UnityEngine;

namespace Interactable
{
    public class Debris : Entities, IDestructable
    {
        protected override void Start()
        {
            canBeAttacked = true;
        }

        public override void OnClicked()
        {
            Debug.Log("I am a debris look at me!");
        }
    }
}
