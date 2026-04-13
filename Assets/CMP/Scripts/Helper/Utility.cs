using UnityEngine;

namespace CMP.Scripts.Helper
{
	public static class Utility
	{
		public static Vector2Int InputDirectionToDirection(InputDirection direction)
		{
			switch (direction)
			{
				case InputDirection.Up:
					return Vector2Int.up;
				case InputDirection.Down:
					return Vector2Int.down;
				case InputDirection.Left:
					return Vector2Int.left;
				case InputDirection.Right:
					return Vector2Int.right;
				default:
					return Vector2Int.zero;
			}
		}
	}
}