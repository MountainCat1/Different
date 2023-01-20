using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GridCharacterController
{
    public static PlayerController Instance { get; private set; }

    public static event Action<Vector2Int> CharacterInteractEvent;


    public bool Interacted { get; set; }

    protected override void Awake()
    {
        base.Awake();
        if (Instance)
        {
            Destroy(gameObject);
            Debug.LogError("Singeleton duplicated!");
            return;
        }
        Instance = this;
    }

    protected override void Update()
    {
        base.Update();


        if (DialogueManager.Instance.Active // If there is a dialogue active, player cannot move nor interact
            || SaveManager.Instance.saveGamePopup.Active // same, if there is a popup, do NOT accept any input
            || GameManager.Instance.LoadingScene // if the new scene is being loaded, character should not move
            )
        {
            Interacted = false;
            return;
        }

        Movement();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
            Interacted = true;
        }
        else
        {
            Interacted = false;
        } 
    }

    private void Interact()
    {
        Vector2Int targetPos = gridPosition + GetVectorFromDirection(direction);
        CharacterInteractEvent?.Invoke(targetPos);
    }

    void Movement()
    {
        Direction? direction = null;
        if(Input.GetKey(KeyCode.W))
            direction = Direction.Up;
        if (Input.GetKey(KeyCode.D))
            direction = Direction.Right;
        if (Input.GetKey(KeyCode.A))
            direction = Direction.Left;
        if (Input.GetKey(KeyCode.S))
            direction = Direction.Down;

        if(direction != null)
        {
            TryGo((Direction)direction);
        }
    }
}
