using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacerAnimator))]
[RequireComponent (typeof(SpriteRenderer))]
public class GridCharacterController : MonoBehaviour
{
    public static event Action<Vector2Int> TryGoEvent;
    public static event Action<Vector2Int> CharacterChangePosEvent;

    public const float GridCellWidth = 1f;
    public const int baseSpriteOrder = 50;
    public const int spriteOrderlayerWidth = 5;
    public const float walkingSpeedMultipler = 3f;
    public int spriteOrderOffset = 0;

    public enum Direction { Left, Right, Up, Down, Null }


    public float speed;
    public Vector2Int gridPosition;
    public Direction direction;

    private CharacerAnimator animator;

    public bool Moving { get => Vector2.Distance(GetRealPos(gridPosition), transform.position) != 0f; }

    protected virtual void Awake()
    {
        animator = GetComponent<CharacerAnimator>();
    }

    protected virtual void Start()
    {
        UpdateSpriteOrder();
    }

    protected virtual void Update()
    {
        // if(Moving && (Vector2.Distance(GetRealPos(gridPosition), transform.position) != 0f))
        // {
        //     Moving = false;
        //     EnteredTile();
        //}
    }

    protected virtual void LateUpdate()
    {
        UpdateAnimator();


        if (Moving)
        {
            Move();
        }
    }
    protected virtual void EnteredTile()
    {
    }

    private void UpdateAnimator()
    {
        if (Moving)
        {
            animator.SetAnimation(CharacerAnimator.Animation.Walk);
        }
        else
        {
            animator.SetAnimation(CharacerAnimator.Animation.Idle);
        }
    }

    private void Move()
    {
        Vector2 realPos = GetRealPos(gridPosition);

        float step = speed * Time.deltaTime * walkingSpeedMultipler;
        transform.position = Vector2.MoveTowards(transform.position, realPos, step);

        if(Vector2.Distance(transform.position, realPos) == 0)
        {
            EnteredTile();
        }
    }
    public void Teleport(Vector2Int pos)
    {
        transform.position = (Vector2)pos * GridCellWidth;
        gridPosition = pos;
    }
    public bool TryGo(Vector2Int move)
    {
        if (Moving)
            return false;

        TryGoEvent?.Invoke(gridPosition+move);

        // Check if there are any triggers on a tile, if yes, activate
        List<TriggerCollider> triggerColliders = GridMap.Instance.GetTriggers(gridPosition+move);
        foreach (var item in triggerColliders)
        {
            item.CharacterTryEnter(this);
        }

        direction = GetDirectionFromVector(move);

        if (!GridMap.Instance.CanWalk(move + gridPosition))
            return false;

        Go(move);
        return true;
    }
    public bool TryGo(Direction direction)
    {
        Vector2Int move = GetVectorFromDirection(direction);
        return TryGo(move);
    }
    public void Go(Vector2Int move)
    {
        if (move.x < 0)
            animator.flipped = true;
        else if (move.x > 0)
            animator.flipped = false;

        gridPosition += move;
        UpdateSpriteOrder();

        // Check if there are any triggers on a tile, if yes, activate
        List<TriggerCollider> triggerColliders = GridMap.Instance.GetTriggers(gridPosition);
        foreach (var item in triggerColliders)
        {
            item.CharacterEnter(this);
        }

        CharacterChangePosEvent?.Invoke(gridPosition);
    }
    private Vector2 GetRealPos(Vector2Int pos)
    {
        return new Vector2(pos.x * GridCellWidth, pos.y * GridCellWidth);
    }

    private void UpdateSpriteOrder()
    {
        animator.RedererSortingOrder = CalculateSortingOrder(gridPosition.y, spriteOrderOffset);
    }

    public static Vector2Int GetVectorFromDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return new Vector2Int(-1,  0);
            case Direction.Right:
                return new Vector2Int( 1,  0);
            case Direction.Up:
                return new Vector2Int( 0,  1);
            case Direction.Down:
                return new Vector2Int( 0, -1);
            default:
                throw new ArgumentException();
        }
    }
    public static Direction GetDirectionFromVector(Vector2Int direction)
    {
        if (direction.x > 0)
            return Direction.Right;
        if (direction.x < 0)
            return Direction.Left;
        if (direction.y > 0)
            return Direction.Up;
        if (direction.y < 0)
            return Direction.Down;

        return Direction.Null;
    }

    public static int CalculateSortingOrder(int yPosition, int spriteOrderOffset)
    {
        return -yPosition * spriteOrderlayerWidth + baseSpriteOrder + spriteOrderOffset;
    }

    protected static Vector2Int GetGridPosFromRealPos(Vector2 pos)
    {
        return Vector2Int.RoundToInt(pos / GridCellWidth);
    }
}
