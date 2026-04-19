using System;
using UnityEngine;
using UnityEngine.UI;

namespace CMP.Scripts
{
    public enum Direction
    {
        None,
        Left,
        Right,
        Up,
        Down
    }
    
    //I feel like input could do better with an queue implementation
    //new inputs go in to queue and has life time of 0.1 seconds
    //need to try and play but don't have time
    public class InputManager : MonoBehaviour
    {
        public Button LeftButton;
        public Button RightButton;
        public Button UpButton;
        public Button DownButton;

        Direction _currentDirection;
        
        public Action<Direction> OnDirectionChanged;
        
        private void Awake()
        {
            LeftButton.onClick.AddListener(() => InputPressed(Direction.Left));
            RightButton.onClick.AddListener(() => InputPressed(Direction.Right));
            UpButton.onClick.AddListener(() => InputPressed(Direction.Up));
            DownButton.onClick.AddListener(() => InputPressed(Direction.Down));
        }

        private void OnDestroy()
        {
            LeftButton.onClick.RemoveAllListeners();
            RightButton.onClick.RemoveAllListeners();
            UpButton.onClick.RemoveAllListeners();
            DownButton.onClick.RemoveAllListeners();
        }
        
        private void InputPressed(Direction direction)
        {
            if (_currentDirection == direction) return;
            _currentDirection = direction;
            OnDirectionChanged?.Invoke(direction);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                InputPressed(Direction.Up);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                InputPressed(Direction.Left);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                InputPressed(Direction.Down);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                InputPressed(Direction.Right);
            }
        }
#endif

        public Direction ConsumeInput()
        {
            var dir = _currentDirection;
            _currentDirection = Direction.None;
            return dir;
        }
        
        public Direction PeekInput()
        {
            return _currentDirection;
        }
    }
}