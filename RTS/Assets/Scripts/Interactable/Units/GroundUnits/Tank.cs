using UnityEngine;

namespace Interactable
{
    public class Tank : GroundUnits
    {
        public override void OnClicked()
        {
            base.OnClicked();
            Debug.Log("I am a tank");
        }
    }
}
