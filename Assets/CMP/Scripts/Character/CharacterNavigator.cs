using System.Collections.Generic;
using System.Threading;
using CMP.Scripts.CellSource;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CMP.Scripts.Character
{
	public class CharacterNavigator : MonoBehaviour
	{
		public CharacterMovement characterMovement;
		private GridData _gridData;

		public void Inject(GridData gridData, float moveDuration)
		{
			_gridData = gridData;
			characterMovement.Inject(gridData, moveDuration);
		}
		
		public async UniTask Navigate(ICellSource cellSource, IReadOnlyList<CellType> movableCells, CancellationToken cancellationToken, bool onlySwitchDirectionOnCorners)
		{
			var oldDirection = Direction.None;
			while (characterMovement.CurrentCell != cellSource.GetCell())
			{
				var direction = GetNextStepTowardsTarget(characterMovement.CurrentCell, cellSource.GetCell(), movableCells, onlySwitchDirectionOnCorners ? oldDirection.Reverse() : Direction.None);
				characterMovement.SetDirection(direction);
				await characterMovement.TryMove(movableCells, cancellationToken);
				if (cancellationToken.IsCancellationRequested) return;
				oldDirection = direction;
			}
		}
		
		private Direction GetNextStepTowardsTarget(Vector2Int current, Vector2Int target, IReadOnlyList<CellType> movableCells, Direction directionToIgnore)
		{
			var directions = GameSettings.DirectionsToCheck;
			Direction bestDirection = Direction.None;
			float bestDistance = float.MaxValue;
			foreach (var dir in directions)
			{
				if (directionToIgnore == dir) continue;
				var nextPos = current + dir.ToVector2Int();
				if (!_gridData.IsCellMovable(nextPos, movableCells)) continue;
				var dist = Vector2Int.Distance(nextPos, target);
				if (!(dist < bestDistance)) continue;
				bestDistance = dist;
				bestDirection = dir;
			}

			if (bestDirection == Direction.None && directionToIgnore != Direction.None)
			{
				var nextPos = current + directionToIgnore.ToVector2Int();
				if (_gridData.IsCellMovable(nextPos, movableCells))
				{
					bestDirection = directionToIgnore;
				}
			}

			if (bestDirection == Direction.None)
			{
				throw new System.Exception($"No valid direction found to move from {current} towards {target}!");
			}

			return bestDirection;
		}
	}
}