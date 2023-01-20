using System;
using UnityEngine;

namespace TCASystem
{
    public class OnInteractTrigger : TriggerObject
    {
        private Vector2Int triggerPos;

        private void OnEnable ()
        {
            triggerPos = CalculatePosGridPos();
            PlayerController.CharacterInteractEvent += OnPlayerInteract;
        }

        private void OnDestroy()
        {
            PlayerController.CharacterInteractEvent -= OnPlayerInteract;
        }

        private void OnPlayerInteract(Vector2Int pos)
        {
            if(pos == triggerPos)
                RunActions();
        }
    }
}