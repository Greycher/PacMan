using System;
using CMP.Scripts.AiStates;
using CMP.Scripts.Character;
using UnityEngine;

namespace CMP.Scripts
{
    public enum GhostState
    {
        InHouse,
        JoiningGame,
        Scatter,
        Chase,
    }

    public class Ghost : MonoBehaviour
    {
        public CharacterNavigator navigator;
        public GhostStateMachine stateMachine;
        
        private GhostState _currentState;
        private GhostBlackboard _blackboard;
        private JoiningGameState _joiningGameState;
        private InHouseState _inHouseState;
        private ScatterState _scatterState;
        private ChaseState _chaseState;
        private bool _chasing;

        public void Inject(GridData gridData, Pacman pacman, Action onPackManSeen)
        {
            navigator.Inject(gridData, GameSettings.AiMovementDuration);
            _blackboard = new GhostBlackboard(gridData, navigator);
            _joiningGameState = new JoiningGameState(_blackboard, OnJoinAction);
            _inHouseState = new InHouseState(_blackboard);
            _scatterState = new ScatterState(_blackboard, pacman, onPackManSeen);
            _chaseState = new ChaseState(_blackboard, pacman);
        }

        public void StartAI()
        {
            _currentState = GhostState.InHouse;
            stateMachine.SetState(_inHouseState);
        }
        
        public void JoinGame()
        {
            _currentState = GhostState.JoiningGame;
            stateMachine.SetState(_joiningGameState);
        }

        private void Scatter()
        {
            _currentState = GhostState.Scatter;
            stateMachine.SetState(_scatterState);
        }
        
        public void Chase()
        {
            _chasing = true;
            if (_currentState != GhostState.Scatter) return;
            ChaseInternal();
        }

        private void ChaseInternal()
        {
            _currentState = GhostState.Chase;
            stateMachine.SetState(_chaseState);
        }

        public void StopAI()
        {
            stateMachine.SetState(null);
        }

        private void OnJoinAction()
        {
            Debug.Log("Ghost joined game!");
            if (_chasing) ChaseInternal();
            else Scatter();
        }
    }
}