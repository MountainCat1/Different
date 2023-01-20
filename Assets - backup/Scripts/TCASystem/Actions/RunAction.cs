using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCASystem
{
    public class RunAction : ActionObject
    {
        public ActionObject action;
        public override void Action()
        {
            action.RunAction();
        }
    }
}

