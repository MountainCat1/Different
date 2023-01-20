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
        [SerializeField] private Dialogue dialogue;

        public override void Action()
        {
            DialogueManager.Instance.InitiateDialogue(dialogue);
        }
    }
}
