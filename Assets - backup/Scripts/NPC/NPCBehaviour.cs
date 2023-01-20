using UnityEngine;

[RequireComponent(typeof(NPCCharacterController))]
public class NPCBehaviour : MonoBehaviour
{
    protected NPCCharacterController characterController;

    protected virtual void Awake()
    {
        characterController = GetComponent<NPCCharacterController>();
    }

    protected virtual void Update()
    {

    }
}
