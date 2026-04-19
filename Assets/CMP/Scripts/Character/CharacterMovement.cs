using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace CMP.Scripts.Character
{
	public class CharacterMovement : MonoBehaviour
	{
		public Transform[] rotationTransforms;
		
		private GridData _gridData;
		private CancellationTokenSource _moveCancellationToken;
		private float _moveSpeed;
		
		public Vector2Int CurrentCell => _gridData.GetClosestCell(transform.position);

		public void Inject(GridData gridData, float moveDuration)
		{
			_gridData = gridData;
			_moveSpeed = 1f / moveDuration;
		}

		public async UniTask<bool> TryMove(IReadOnlyList<CellType> movableCells, CancellationToken cancellationToken)
		{
			var nextCell = CurrentCell + GetDirection().ToVector2Int();
			if (!_gridData.IsCellMovable(nextCell, movableCells)) return false;
			
			var target = new Vector3(nextCell.x, nextCell.y, transform.position.z);
			await transform.DOMove(target, _moveSpeed).SetSpeedBased(true).SetEase(Ease.Linear).WithCancellation(cancellationToken);
			return true;
		}

		public Direction GetDirection()
		{
			return rotationTransforms[0].up.ToDirection();
		}
		
		public void SetDirection(Direction direction)
		{
			foreach (var tr in rotationTransforms)
			{
				tr.up = direction.ToVector3();
			}
		}
		
	}
}