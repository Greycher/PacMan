using System;
using System.Collections.Generic;
using System.Threading;
using CMP.Scripts.Helper;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace CMP.Scripts
{
    public enum GameMode
    {
        Scatter,
        Chase,
        GameOver,
    }

    public class GameManager : MonoBehaviour
    {
        private Pacman _pacman;
        private InputManager _inputManager;
        private GameMode _gameMode = GameMode.Scatter;
        private List<Ghost> _ghosts;

        private void Start()
        {
            var gridData = AssetDatabase.Instance.GridData;
            CreateBackground(gridData);
            AdjustCamera(gridData);
            _inputManager = Instantiate(AssetDatabase.Instance.InputManagerPrefab);
            _pacman = CreatePlayer(_inputManager, gridData);
            _ghosts = CreateGhosts(_pacman, gridData, OnPackManSeen);
            StartGame(CancellationToken.None).Forget(Debug.LogError);
        }
        
        private void CreateBackground(GridData gridData)
        {
            var targetTexture = MapTextureGenerator.Generate(gridData, AssetDatabase.Instance.MapVisualSettings);
            var textureObject = new GameObject("MapTexture");
            textureObject.transform.position = new Vector3(-0.5f, -0.5f, 0f);
            var targetSprite = Sprite.Create(targetTexture, new Rect(0f, 0f, targetTexture.width, targetTexture.height),
                Vector2.zero,AssetDatabase.Instance.MapVisualSettings.pixelsPerCell);
            var spriteRenderer = textureObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = targetSprite;
            spriteRenderer.sortingOrder = -1;
        }
        
        private void AdjustCamera(GridData gridData)
        {
            var mainCamera = Camera.main;
            mainCamera.orthographicSize = gridData.Height + GameSettings.CameraPadding;
            mainCamera.transform.position = new Vector3(gridData.Width / 2f - 0.5f, 0f, -10f);
        }

        private static Pacman CreatePlayer(InputManager inputManager, GridData gridData)
        {
            var pacman = Instantiate(AssetDatabase.Instance.PacmanPrefab);
            pacman.Inject(gridData, inputManager);
            return pacman;
        }

        private static List<Ghost> CreateGhosts(Pacman pacman, GridData gridData, Action onPacManSeen)
        {
            var ghosts = new List<Ghost>();
            var ghostSpawnPositions = gridData.GetCoordsOfCellType(CellType.AiSpawnZone);
            Assert.IsTrue(ghostSpawnPositions.Count >= GameSettings.AiCharacterCount);
            for (int i = 0; i < GameSettings.AiCharacterCount; i++)
            {
                var pos2Int = ghostSpawnPositions[i];
                var pos = new Vector3(pos2Int.x, pos2Int.y, 0);
                var ghost = Instantiate(AssetDatabase.Instance.Ghost, pos, Quaternion.identity);
                ghost.Inject(gridData, pacman, onPacManSeen); 
                ghost.StartAI();
                ghosts.Add(ghost);
            }
            
            return ghosts;
        }

        private async UniTask StartGame(CancellationToken cancellationToken)
        {
            _pacman.StartMoving();
            for (int i = 0; i < _ghosts.Count; i++)
            {
                await UniTask.WaitForSeconds(GameSettings.AiJoinDelays[i], cancellationToken: cancellationToken).
                    SuppressCancellationThrow();
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                _ghosts[i].JoinGame();
            }
        }

        private void OnPackManSeen()
        {
            Debug.Log("Ghost starts chasing...");
            foreach (var ghost in _ghosts)
            {
                ghost.Chase();
            }
        }

        private void Update()
        {
            HandleFailCase();
        }

        private void HandleFailCase()
        {
            if (_gameMode == GameMode.GameOver) return;
            if (DidAnyGhostCatchPackMan())
            {
                OnGameFail();
            }
        }
        
        private bool DidAnyGhostCatchPackMan()
        {
            var pacManPos = _pacman.transform.position;
            foreach (var ghost in _ghosts)
            {
                if (Vector3.Distance(ghost.transform.position, pacManPos) <= GameSettings.CatchDistance)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        private void OnGameFail()
        {
            _gameMode = GameMode.GameOver;
            Debug.Log("Game Fail");
            _pacman.OnFail();
            foreach (var ghost in _ghosts)
            {
                ghost.StopAI();
            }
        }

    }
}