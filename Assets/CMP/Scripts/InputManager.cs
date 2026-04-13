using System;
using UnityEngine;
using UnityEngine.UI;

namespace CMP.Scripts
{
    public enum InputDirection
    {
        None,
        Left,
        Right,
        Up,
        Down
    }
    
    public class InputManager : MonoBehaviour
    {
        public Button LeftButton;
        public Button RightButton;
        public Button UpButton;
        public Button DownButton;

        InputDirection _currentDirection;
        
        private void Awake()
        {
            LeftButton.onClick.AddListener(() =>
            {
                _currentDirection = InputDirection.Left;
            });
            RightButton.onClick.AddListener(() =>
            {
                _currentDirection = InputDirection.Right;
            });
            UpButton.onClick.AddListener(() =>
            {
                _currentDirection = InputDirection.Up;
            });
            DownButton.onClick.AddListener(() =>
            {
                _currentDirection = InputDirection.Down;
            });
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _currentDirection = InputDirection.Up;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                _currentDirection = InputDirection.Left;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                _currentDirection = InputDirection.Down;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                _currentDirection = InputDirection.Right;
            }
        }
#endif

        public InputDirection ConsumeInput()
        {
            var dir = _currentDirection;
            _currentDirection = InputDirection.None;
            return dir;
        }
        
        public InputDirection PeekInput()
        {
            return _currentDirection;
        }
    }
}