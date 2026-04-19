using System.Threading;
using CMP.Scripts.CellSource;
using CMP.Scripts.Character;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CMP.Scripts
{
    public class Pacman : MonoBehaviour, ICellSource
    {
        public CharacterMovement characterMovement;
        
        public Animator Animator;
        private const string FailAnimationName = "FailAnimation";
        
        private CancellationTokenSource _moveCancellationToken;
        private GridData _gridData;
        private InputManager _inputManager;

        private readonly CellType[] _movableCellTypes = { CellType.Empty, CellType.Pacman, CellType.JoinGameCell };
        private bool IsMoving => !_moveCancellationToken?.IsCancellationRequested ?? false;
        
        public void Inject(GridData gridData, InputManager inputManager)
        {
            _gridData = gridData;
            _inputManager = inputManager;
            characterMovement.Inject(gridData, GameSettings.PacmanMovementDuration);
            _inputManager.OnDirectionChanged += OnInputDirectionChanged;
        }

        //To support quick reverse
        private void OnInputDirectionChanged(Direction direction)
        {
            if (!IsMoving || characterMovement.GetDirection() != direction.Reverse())
            {
                return;
            }

            StopMoving();
            StartMoving();
        }

        public void StartMoving()
        {
            _moveCancellationToken = new CancellationTokenSource();
            MoveRoutine(_moveCancellationToken.Token).Forget(Debug.LogError);
        }
        
        public void StopMoving()
        {
            _moveCancellationToken?.Cancel();
        }

        private async UniTask MoveRoutine(CancellationToken cancellationToken)
        {
            while (true)
            {
                HandleRotate();
                if (!await characterMovement.TryMove(_movableCellTypes, cancellationToken))
                { 
                    await UniTask.NextFrame(cancellationToken: cancellationToken).SuppressCancellationThrow();
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private void HandleRotate()
        {
            var direction = _inputManager.PeekInput();
            if (direction == Direction.None)
            {
                return;
            }
            
            var targetCell = characterMovement.CurrentCell + direction.ToVector2Int();
            if (_gridData.IsCellMovable(targetCell, _movableCellTypes))
            {
                _inputManager.ConsumeInput();
                characterMovement.SetDirection(direction);
            }
        }

        public void OnFail()
        {
            Animator.Play(FailAnimationName);
            StopMoving();
        }

        public Vector2Int GetCell()
        {
            return characterMovement.CurrentCell;
        }

        private void OnDestroy()
        {
            if (_inputManager)
            {
                _inputManager.OnDirectionChanged -= OnInputDirectionChanged;
            }
        }
    }
}