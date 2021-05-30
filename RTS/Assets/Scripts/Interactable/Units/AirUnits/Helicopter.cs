using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactable
{

    public class Helicopter : AirUnits
    {

        public override void OnClicked()
        {
            base.OnClicked();
            Debug.Log("I can fly and fire at things :D");
        }
    }
}
