using System.Collections.Generic;
using System.Threading;
using CMP.Scripts.AiStates;
using CMP.Scripts.CellSource;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CMP.Scripts.Character
{
	public class CharacterNavigator : MonoBehaviour
	{
		public CharacterMovement characterMovement;

		private Vector2Int[] _directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
		private GridData _gridData;

		public void Inject(GridData gridData, float moveDuration)
		{
			_gridData = gridData;
			characterMovement.Inject(gridData, moveDuration);
		}
		
		public async UniTask Navigate(ICellSource cellSource, IReadOnlyList<CellType> movableCells, CancellationToken cancellationToken)
		{
			while (characterMovement.CurrentCell != cellSource.GetCell())
			{
				var direction = GetNextStepTowardsTarget(characterMovement.CurrentCell, cellSource.GetCell(), movableCells);
				characterMovement.SetDirection(direction);
				await characterMovement.TryMove(movableCells, cancellationToken);
				if (cancellationToken.IsCancellationRequested) return;
			}
		}
		
		private Vector2Int GetNextStepTowardsTarget(Vector2Int current, Vector2Int target, IReadOnlyList<CellType> movableCells)
		{
			Vector2Int bestDirection = _directions[0];
			float bestDistance = float.MaxValue;
			foreach (var dir in _directions)
			{
				var nextPos = current + dir;
				if (_gridData.IsCellMovable(nextPos, movableCells))
				{
					var dist = Vector2Int.Distance(nextPos, target);
					if (dist < bestDistance)
					{
						bestDistance = dist;
						bestDirection = dir;
					}
				}
			}

			return bestDirection;
		}
	}
}