using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCASystem
{
    public class DebugAction : ActionObject
    {
        public override void Action()
        {
            Debug.Log($"Debug action! {name}");
        }
    }

}
