using System.Collections.Generic;
using System.Threading;
using CMP.Scripts.Character;
using CMP.Scripts.Helper;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CMP.Scripts
{
    public class Pacman : MonoBehaviour
    {
        public CharacterMovement characterMovement;
        
        public Animator Animator;
        private const string FailAnimationName = "FailAnimation";
        
        private CancellationTokenSource _moveCancellationToken;
        private GridData _gridData;
        private InputManager _inputManager;

        private readonly List<CellType> _movableCellTypes = new() { CellType.Empty, CellType.Pacman, CellType.JoinGameCell };

        public void Inject(GridData gridData, InputManager inputManager)
        {
            _gridData = gridData;
            _inputManager = inputManager;
            characterMovement.Inject(gridData, GameSettings.PacmanMovementDuration);
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
                if (!await characterMovement.TryMove(cancellationToken, _movableCellTypes))
                { 
                    await UniTask.WaitForSeconds(GameSettings.PacmanMovementDuration, 
                        cancellationToken: cancellationToken).SuppressCancellationThrow();
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private void HandleRotate()
        {
            var input = _inputManager.PeekInput();
            if (input == InputDirection.None)
            {
                return;
            }
            
            var direction = Utility.InputDirectionToDirection(input);
            var targetCell = characterMovement.CurrentGridPosition + direction;
            if (_gridData.IsCellMovable(targetCell, _movableCellTypes))
            {
                _inputManager.ConsumeInput();
                characterMovement.SetDirection(direction);
            }
        }
    }
}