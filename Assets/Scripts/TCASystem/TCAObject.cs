using UnityEngine;

namespace TCASystem
{
    public class TCAObject : MonoBehaviour
    {
        protected Vector2Int CalculatePosGridPos()
        {
            return Vector2Int.RoundToInt(transform.position / GridCharacterController.GridCellWidth);
        }
    }
}
