using System;
using System.Collections.Generic;
using UnityEngine;

namespace TCASystem
{
    public class SetActiveGameObjectAction : ActionObject
    {
        [SerializeField] private GameObject targetGameObject;
        [SerializeField] private bool value = false;
        public override void Action()
        {
            targetGameObject.SetActive(value);
        }
    }

}
