using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCharacterController : GridCharacterController
{
    protected override void Start()
    {
        gridPosition = GetGridPosFromRealPos(transform.position);
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        /*
        if (!Moving)
        {
            var goalPos = NavigationManager.Instance.GetStupidPath(gridPosition, PlayerController.Instance.gridPosition).Position;
            var move =  goalPos - gridPosition;
            TryGo(move);
        }
        */
    }
}
