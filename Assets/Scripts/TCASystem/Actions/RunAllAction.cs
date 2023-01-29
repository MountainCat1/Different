using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TCASystem
{

    public class RunAllAction : ActionObject
    {
        [SerializeField] private GameObject actionsContainer;
        
        private RunAction[] actions;

        public override void Action()
        {
            foreach (RunAction action in actions)
            {
                action.RunAction();
            }
        }

        private void Awake()
        {
            if (actionsContainer == null)
                actionsContainer = gameObject;

            actions = actionsContainer.GetComponentsInChildren<RunAction>();
        }
    }
}
