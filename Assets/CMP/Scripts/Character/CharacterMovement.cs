using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CMP.Scripts.Character
{
	public class CharacterMovement : MonoBehaviour
	{
		public Transform[] rotationTransforms;
		
		private GridData _gridData;
		private CancellationTokenSource _moveCancellationToken;
		private float _moveDuration;
		
		public Vector2Int CurrentCell => _gridData.GetClosestCell(transform.position);

		public void Inject(GridData gridData, float moveDuration)
		{
			_gridData = gridData;
			_moveDuration = moveDuration;
		}

		public async UniTask<bool> TryMove(IReadOnlyList<CellType> movableCells, CancellationToken cancellationToken)
		{
			var nextCell = CurrentCell + GetDirection();
			if (!_gridData.IsCellMovable(nextCell, movableCells)) return false;
			
			var target = new Vector3(nextCell.x, nextCell.y, transform.position.z);
			await transform.DOMove(target, _moveDuration).ToUniTask(cancellationToken: cancellationToken).SuppressCancellationThrow();
			return true;
		}

		public Vector2Int GetDirection()
		{
			var up = rotationTransforms[0].up;
			return new Vector2Int(Mathf.RoundToInt(up.x), Mathf.RoundToInt(up.y));
		}
		
		public void SetDirection(Vector2Int direction)
		{
			foreach (var tr in rotationTransforms)
			{
				tr.up = new Vector3(direction.x, direction.y, 0f);
			}
		}
		
	}
}