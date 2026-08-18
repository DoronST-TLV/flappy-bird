using System;
using UnityEngine;

namespace FlappyBird
{
    public enum GameState
    {
        Ready,
        Playing,
        GameOver
    }

    /// <summary>
    /// Owns the game loop and the single source of truth for game state and score.
    /// Access from anywhere via GameManager.Instance.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Player _player;
        [SerializeField] private PipeSpawner _pipeSpawner;

        public GameState State { get; private set; } = GameState.Ready;
        public int Score { get; private set; }
        public int HighScore { get; private set; }

        public event Action<GameState> StateChanged;
        public event Action<int> ScoreChanged;

        private const string HighScoreKey = "flappy_high_score";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            SetState(GameState.Ready);
        }

        private void Update()
        {
            if (State == GameState.Ready && InputPressed())
            {
                StartGame();
            }
            else if (State == GameState.GameOver && InputPressed())
            {
                Restart();
            }
        }

        private static bool InputPressed()
        {
            return Input.GetKeyDown(KeyCode.Space)
                   || Input.GetMouseButtonDown(0)
                   || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        }

        public void StartGame()
        {
            Score = 0;
            ScoreChanged?.Invoke(Score);
            SetState(GameState.Playing);
        }

        public void AddScore(int amount)
        {
            if (State != GameState.Playing)
            {
                return;
            }

            Score += amount;
            ScoreChanged?.Invoke(Score);
        }

        public void GameOver()
        {
            if (State != GameState.Playing)
            {
                return;
            }

            if (Score > HighScore)
            {
                HighScore = Score;
                PlayerPrefs.SetInt(HighScoreKey, HighScore);
                PlayerPrefs.Save();
            }

            SetState(GameState.GameOver);
        }

        public void Restart()
        {
            _player.ResetPlayer();
            _pipeSpawner.ResetSpawner();
            SetState(GameState.Ready);
        }

        private void SetState(GameState newState)
        {
            State = newState;
            StateChanged?.Invoke(State);
        }
    }
}
