using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CMP.Scripts.AiStates
{
    public class ScatterState : GhostState
    {
        private readonly List<CellType> _movableCellTypes = new() { CellType.Empty, CellType.JoinGameCell, CellType.Pacman};
        private Vector2Int[] _directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        private CancellationTokenSource _cancellationTokenSource;
        private List<Vector2Int> _possibleDirections = new();
        private readonly Pacman _pacman;
        private readonly Action _onPacManSeen;

        public ScatterState(GhostBlackboard blackboard, Pacman pacman, Action onPacManSeen) : base(blackboard)
        {
            _pacman = pacman;
            _onPacManSeen = onPacManSeen;
        }
        
        public override void OnEnter()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Scatter(_cancellationTokenSource.Token).Forget(Debug.LogError);
        }

        private async UniTask Scatter(CancellationToken cancellationToken)
        {
            var characterMovement = GhostBlackboard.CharacterNavigator.characterMovement;
            var oldDirection = Vector2Int.zero;
            while (true)
            {
                _possibleDirections.Clear();
                var currentCell = characterMovement.CurrentCell;
                foreach (var direction in _directions)
                {
                    if (direction == -oldDirection) continue;
                    if (GhostBlackboard.GridData.IsCellMovable(currentCell + direction, _movableCellTypes))
                    {
                        _possibleDirections.Add(direction);
                    }   
                }

                if (_possibleDirections.Count == 0)
                {
                    _possibleDirections.Add(-oldDirection);
                }
                
                var newDirection = _possibleDirections[Random.Range(0, _possibleDirections.Count)];
                characterMovement.SetDirection(newDirection);
                if (!await characterMovement.TryMove(_movableCellTypes, cancellationToken: cancellationToken))
                {
                    throw new Exception($"Ghost can not find a direction to move in {nameof(ScatterState)} state!");
                }

                if (cancellationToken.IsCancellationRequested) return;
                oldDirection = newDirection;
            }
        }

        public override void Update()
        {
            if (HasLineOfSightToPacman())
            {
                _onPacManSeen?.Invoke();
            }
        }

        private bool HasLineOfSightToPacman()
        {
            return Random.value < 0.1f;
        }

        public override void OnExit()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
