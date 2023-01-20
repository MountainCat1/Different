using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCASystem
{
    public class OnInteractWithColliderTrigger : TriggerObject
    {
        public TriggerCollider triggerCollider;

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
            PlayerController.CharacterInteractEvent += OnPlayerInteract;
        }
        private void OnDisable()
        {
            PlayerController.CharacterInteractEvent -= OnPlayerInteract;
        }

        private void OnPlayerInteract(Vector2Int pos)
        {
            if (triggerCollider.GetCollisions().Contains(pos))
            {
                RunActions();
            }
        }
    }

}
