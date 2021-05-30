using UnityEngine;

namespace Interactable
{
    public class Debris : Entities
    {
        protected override void Start()
        {
            base.Start();
            canBeAttacked = true;
        }

        public override void OnClicked()
        {
            base.OnClicked();
            Debug.Log("I am a debris look at me!");
        }
    }
}
