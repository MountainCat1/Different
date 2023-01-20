using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCASystem {
    public class SaveGameAction : ActionObject
    {
        public override void Action()
        {
            SaveManager.Instance.ShowPopup();
        }
    }
}


