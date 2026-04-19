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
        private CancellationTokenSource _cancellationTokenSource;
        private List<Direction> _possibleDirections = new();
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
            var oldDirection = Direction.None;
            while (true)
            {
                _possibleDirections.Clear();
                var currentCell = characterMovement.CurrentCell;
                foreach (var direction in GameSettings.DirectionsToCheck)
                {
                    if (direction == oldDirection.Reverse()) continue;
                    if (GhostBlackboard.GridData.IsCellMovable(currentCell + direction.ToVector2Int(), _movableCellTypes))
                    {
                        _possibleDirections.Add(direction);
                    }   
                }

                if (_possibleDirections.Count == 0)
                {
                    _possibleDirections.Add(oldDirection.Reverse());
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
            if (Vector3.Distance(_pacman.transform.position, 
                    GhostBlackboard.CharacterNavigator.transform.position) > GameSettings.LineOfSightRange)
            {
                return false;
            }
            
            var characterMovement = GhostBlackboard.CharacterNavigator.characterMovement;
            var ghostCell = characterMovement.CurrentCell;
            var ghostDir = characterMovement.GetDirection().ToVector2Int();
            var pacCell = _pacman.characterMovement.CurrentCell;

            if (ghostDir.x != 0)
            {
                if (pacCell.y != ghostCell.y) return false;
                
                var directionSign = (int)Mathf.Sign(ghostDir.x);
                var pacmanSign = pacCell.x > ghostCell.x ? 1 : -1;
                
                if (directionSign != pacmanSign) return false;
                
                var step = directionSign > 0 ? 1 : -1;
                for (var x = ghostCell.x + step; x != pacCell.x; x += step)
                {
                    if (GhostBlackboard.GridData.GetCellAt(x, ghostCell.y) == CellType.Wall)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                if (pacCell.x != ghostCell.x) return false;
                
                var directionSign = (int)Mathf.Sign(ghostDir.y);
                var pacmanSign = pacCell.y > ghostCell.y ? 1 : -1;
                
                if (directionSign != pacmanSign) return false;

                var step = directionSign > 0 ? 1 : -1;
                for (var y = ghostCell.y + step; y != pacCell.y; y += step)
                {
                    if (GhostBlackboard.GridData.GetCellAt(ghostCell.x, y) == CellType.Wall)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public override void OnExit()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
