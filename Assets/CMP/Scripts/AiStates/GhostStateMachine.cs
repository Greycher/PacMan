using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class GhostStateMachine : MonoBehaviour
    {
        private GhostState _currentState;

        public void SetState(GhostState newState)
        {
            _currentState?.OnExit();
            _currentState = newState;
            _currentState?.OnEnter();
        }
        
        private void Update()
        {
            _currentState?.Update();
        }
        
        private void OnDestroy()
        {
            SetState(null);
        }
    }
}
