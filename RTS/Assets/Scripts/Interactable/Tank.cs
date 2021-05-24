using UnityEngine;

namespace Interactable
{
    public class Tank : Units, IInteractable
    {
        public void OnClicked()
        {
            Debug.Log("I have been called" + name);
        }
    }
}
