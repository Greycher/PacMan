using System;
using System.Threading;
using CMP.Scripts.CellSource;
using CMP.Scripts.Character;

namespace CMP.Scripts.AiStates
{
    public class JoiningGameState : GhostState
    {
        private readonly CharacterNavigator _characterNavigator;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly CellType[] _movableCellTypes = { CellType.Empty, CellType.AiGate, CellType.JoinGameCell };
        private readonly Action _onJoinAction;

        public JoiningGameState(GhostBlackboard blackboard, Action onJoinAction) : base(blackboard)
        {
            _onJoinAction = onJoinAction;
        }

        public async override void OnEnter()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var targetCell = GhostBlackboard.GridData.GetCoordsOfCellType(CellType.JoinGameCell)[0];
            var token = _cancellationTokenSource.Token;
            await GhostBlackboard.CharacterNavigator.Navigate(new StaticCellSource(targetCell), _movableCellTypes, token, false);
            if (token.IsCancellationRequested) return;
            _onJoinAction?.Invoke();
        }

        public override void Update() {}
        public override void OnExit()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
