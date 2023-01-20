using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCASystem {
    public abstract class CheckObject : ActionObject
    {
        public enum LogicalType
        {
            And,
            Or,
            Not,
            Dif,
            Same
        }

        public List<ActionObject> actions = new List<ActionObject>() { null }; 
        public override void Action()
        {
            if (Check())
            {
                RunActions();
            }
        }

        public void RunActions()
        {
            foreach (ActionObject actionObject in actions)
            {
                if (actionObject != null)
                    actionObject.RunAction();
                else
                    Debug.LogWarning("Action Object assigned to check is null");
            }
        }

        public abstract bool Check();
    }
}

