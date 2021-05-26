using UnityEngine;

namespace Interactable
{
    public class Units : MonoBehaviour, IInteractable
    {
        public int hitPoints;
        public string nameOfUnit;
        public virtual void OnClicked()
        {
            Debug.Log("This is the unit: " + nameOfUnit);
        }
    }
}
