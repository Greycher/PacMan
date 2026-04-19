using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class InHouseState : GhostState
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly CellType[] _movableCellTypes = { CellType.Empty, CellType.AiSpawnZone};

        public InHouseState(GhostBlackboard blackboard) : base(blackboard) {}

        public override void OnEnter()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            MoveUpAndDown(_cancellationTokenSource.Token).Forget(Debug.LogError);
        }

        private async UniTask MoveUpAndDown(CancellationToken token)
        {
            var characterMovement = GhostBlackboard.CharacterNavigator.characterMovement;
            bool up = true;
            while (true)
            {
                var direction = up ? Vector2Int.up : Vector2Int.down;
                characterMovement.SetDirection(direction);
                if (! await characterMovement.TryMove(_movableCellTypes, token))
                {
                    throw new Exception($"Ghost can't move up and down in {nameof(InHouseState)} state!");
                }
                if (token.IsCancellationRequested) return;
                up = !up;
            }
        }

        public override void Update() {}
        
        public override void OnExit()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
