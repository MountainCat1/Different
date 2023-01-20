using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TCASystem
{
    public class TriggerObject : TCAObject
    {
        public List<ActionObject> Actions = new List<ActionObject>() { };


        protected void RunActions()
        {
            if(Actions.Count == 0)
            {
                Actions = GetComponents<ActionObject>().ToList();
            }
            foreach (ActionObject actionObject in Actions)
            {
                if (actionObject != null)
                    actionObject.RunAction();
                else
                    Debug.LogWarning("Action Object assigned to trigger is null");
            }
        }

        
    }
}
