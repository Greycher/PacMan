using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts
{
    public static class Extensions
    {
        public static bool GetIsMovable(this CellType cellType)
        {
            return cellType is CellType.Pacman or CellType.Empty or CellType.JoinGameCell;
        }

        public static List<Vector2Int> GetNeighbours(this Vector2Int cellCoords)
        {
            return new List<Vector2Int>
            {
                cellCoords + Vector2Int.left, 
                cellCoords + Vector2Int.right, 
                cellCoords + Vector2Int.up,
                cellCoords + Vector2Int.down
            };
        }

        public static Vector2Int ToVector2Int(this InputDirection cellType)
        {
            switch (cellType)
            {
                case InputDirection.None:
                    return Vector2Int.zero;
                case InputDirection.Left:
                    return Vector2Int.left;
                case InputDirection.Right:
                    return Vector2Int.right;
                case InputDirection.Up:
                    return Vector2Int.up;
                case InputDirection.Down:
                    return Vector2Int.down;
            }

            Debug.Assert(false);
            return Vector2Int.zero;
        }

        public static InputDirection Reverse(this InputDirection direction)
        {
            return direction switch
            {
                InputDirection.Left => InputDirection.Right,
                InputDirection.Right => InputDirection.Left,
                InputDirection.Up => InputDirection.Down,
                InputDirection.Down => InputDirection.Up,
                _ => InputDirection.None
            };
        }
        
        public static InputDirection ToDirection(this Vector2Int vector)
        {
            if (vector == Vector2Int.left)
            {
                return InputDirection.Left;
            }
            if (vector == Vector2Int.right)
            {
                return InputDirection.Right;
            }
            if (vector == Vector2Int.up)
            {
                return InputDirection.Up;
            }
            if (vector == Vector2Int.down)
            {
                return InputDirection.Down;
            }

            return InputDirection.None;
        }

        public static Quaternion ToQuaternion(this InputDirection dir)
        {
            switch (dir)
            {
                case InputDirection.Left:
                    return Quaternion.Euler(0, 0, 90);
                case InputDirection.Right:
                    return Quaternion.Euler(0, 0, -90);
                case InputDirection.Up:
                    return Quaternion.Euler(0, 0, 0);
                case InputDirection.Down:
                    return Quaternion.Euler(0, 0, 180);
            }
            
            return Quaternion.identity;
        }
    }
}