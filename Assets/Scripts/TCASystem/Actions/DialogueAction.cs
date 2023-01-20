using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TCASystem
{
    public class DialogueAction : ActionObject
    {
        [SerializeField] private DialogueContainer dialogueContainer;

        public override void Action()
        {
            DialogueManager.Instance.InitiateDialogue(dialogueContainer);
        }
    }
}
