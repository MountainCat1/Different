using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MapObject : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sortingOrder = GridCharacterController.CalculateSortingOrder(
            Mathf.RoundToInt(transform.position.y / GridCharacterController.GridCellWidth),
            0);
    }
}