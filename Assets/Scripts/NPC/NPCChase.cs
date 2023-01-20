using System.Collections.Generic;
using UnityEngine;

public class NPCChase : NPCBehaviour
{
    protected override void Update()
    {
        base.Update();

        if (!characterController.Moving)
        {
            GridTile start = GridMap.Instance.Tiles[characterController.gridPosition];
            GridTile goal = GridMap.Instance.Tiles[PlayerController.Instance.gridPosition];

            var path = GetPath(start, goal);

            if(path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(path[i].RealPosition, path[i + 1].RealPosition);
                }

                characterController.TryGo(path[0].Position - characterController.gridPosition);
            }
                
        }
    }
    private List<GridTile> GetPath(GridTile start, GridTile goal)
    {
        return NavigationManager.Instance.GetPath(start, goal);
    }

}