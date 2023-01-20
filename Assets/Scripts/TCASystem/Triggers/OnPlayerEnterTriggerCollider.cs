
using UnityEngine;

namespace TCASystem
{
    [RequireComponent(typeof(TriggerCollider))]
    public class OnPlayerEnterTriggerCollider : TriggerObject
    {
        [SerializeField] private TriggerCollider triggerCollider;
        [SerializeField] private bool doNotCheckCollision = true;

        private void Awake()
        {
            triggerCollider = GetComponent<TriggerCollider>();
        }

        private void OnEnable()
        {
            if (doNotCheckCollision)
            {
                triggerCollider.OnPlayerCharacterTryEnter += EventHandler;
            }
            else
            {
                triggerCollider.OnPlayerCharacterEnter += EventHandler;
            }
        }
        private void OnDisable()
        {
            triggerCollider.OnPlayerCharacterTryEnter -= EventHandler;
            triggerCollider.OnPlayerCharacterEnter -= EventHandler;
        }

        private void EventHandler(GridCharacterController character)
        {
            RunActions();
        }
    }
}
