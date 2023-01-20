using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TCASystem
{
    public class RunOneAction : ActionObject
    {
        [SerializeField] private GameObject actionsContainer;
        
        private RunAction[] actions;

        public override void Action()
        {
            foreach (RunAction runAction in actions) // AAAAAAAAA KURWAA, co to za jean gówno PROSZĘ 
                                                     // zmień to potem bo to jest kurwa dramat a nie
                                                     // kod, za takie coś to się powinno do więzienia iść
            {
                bool ok = true;
                if (runAction.action is CheckObject)
                {
                    
                    CheckObject checkComponent = (runAction.action as CheckObject);

                    if (!checkComponent.Check())
                    {
                        ok = false;
                    }
                    while (checkComponent.actions[0] is CheckObject)
                    {
                        checkComponent = checkComponent.actions[0] as CheckObject;
                        if (!checkComponent.Check())
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (ok)
                    {
                        checkComponent.RunActions();
                        break;
                    }
                }
                runAction.RunAction();
            }
        }

        private void Awake()
        {
            if (actionsContainer == null)
                actionsContainer = gameObject;

            if (actions == null)
                actions = actionsContainer.GetComponentsInChildren<RunAction>();
        }
    }
}
