using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class ChaseState : GhostState
    {
        private readonly List<CellType> _movableCellTypes = new() { CellType.Empty, CellType.JoinGameCell, CellType.Pacman};
        private CancellationTokenSource _cancellationTokenSource;
        private readonly Pacman _pacman;

        public ChaseState(GhostBlackboard blackboard, Pacman pacman) : base(blackboard)
        {
            _pacman = pacman;
        }
        
        public override void OnEnter()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            GhostBlackboard.CharacterNavigator.
                Navigate(_pacman, _movableCellTypes, _cancellationTokenSource.Token, true).Forget(Debug.LogError);
        }
        
        public override void Update() { }
        
        public override void OnExit()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
