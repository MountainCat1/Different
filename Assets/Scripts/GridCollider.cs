using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCollider : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(1, 1);
    public bool centeredX = true;
    public bool centeredY = false;

    public event Action<GridCharacterController> OnCharacterEnter;
    public event Action<GridCharacterController> OnPlayerCharacterEnter;
    public event Action<GridCharacterController> OnCharacterTryEnter;
    public event Action<GridCharacterController> OnPlayerCharacterTryEnter;

    public Vector2Int offset = new Vector2Int(0, 0);

    public virtual HashSet<Vector2Int> GetCollisions()
    {
        HashSet<Vector2Int> ret = new HashSet<Vector2Int>();

        Vector2Int pos = GetPosition();

        if (centeredX)
        {
            for (int x = -size.x / 2; x < size.x - size.x / 2; x++)
            {
                if (centeredY)
                {
                    for (int y = -size.y / 2; y < size.y - size.y / 2; y++)
                    {
                        ret.Add(new Vector2Int(x, y) + pos);
                    }
                }
                else
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        ret.Add(new Vector2Int(x, y) + pos);
                    }
                }
            }
        }
        else
        {
            for (int x = 0; x < size.x; x++)
            {
                if (centeredY)
                {
                    for (int y = -size.y / 2; y < size.y - size.y / 2; y++)
                    {
                        ret.Add(new Vector2Int(x, y) + pos);
                    }
                }
                else
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        ret.Add(new Vector2Int(x, y) + pos);
                    }
                }
            }
        }

        return ret;
    }

    protected virtual Vector2Int GetPosition()
    {
        return Vector2Int.RoundToInt(transform.position / GridCharacterController.GridCellWidth) + offset;
    }

    /// <summary>
    /// Method activated from GridCharacterController, when entering a collider.
    /// Should work ONLY when collider value of "bool trigger" is set to "true"
    /// </summary>
    /// <param name="gridCharacterController"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void CharacterEnter(GridCharacterController gridCharacterController)
    {
        OnCharacterEnter?.Invoke(gridCharacterController);
        if (gridCharacterController is PlayerController)
            OnPlayerCharacterEnter?.Invoke(gridCharacterController as PlayerController);
    }

    /// <summary>
    /// Method activated from GridCharacterController, when is trying entering a collider.
    /// Should work ONLY when collider value of "bool trigger" is set to "true"
    /// </summary>
    /// <param name="gridCharacterController"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void CharacterTryEnter(GridCharacterController gridCharacterController)
    {
        OnCharacterTryEnter?.Invoke(gridCharacterController);
        if (gridCharacterController is PlayerController)
            OnPlayerCharacterTryEnter?.Invoke(gridCharacterController as PlayerController);
    }
}
