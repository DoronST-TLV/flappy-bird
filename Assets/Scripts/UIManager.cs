using UnityEngine;
using TMPro;

namespace FlappyBird
{
    /// <summary>
    /// Subscribes to GameManager events and drives the UGUI Canvas: live score,
    /// the "tap to start" prompt, and the game-over panel with final/high score.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] private TMP_Text _scoreText;

        [Header("Panels")]
        [SerializeField] private GameObject _readyPanel;
        [SerializeField] private GameObject _gameOverPanel;

        [Header("Game Over")]
        [SerializeField] private TMP_Text _finalScoreText;
        [SerializeField] private TMP_Text _highScoreText;

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StateChanged += HandleStateChanged;
                GameManager.Instance.ScoreChanged += HandleScoreChanged;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StateChanged -= HandleStateChanged;
                GameManager.Instance.ScoreChanged -= HandleScoreChanged;
            }
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                HandleStateChanged(GameManager.Instance.State);
                HandleScoreChanged(GameManager.Instance.Score);
            }
        }

        private void HandleStateChanged(GameState state)
        {
            if (_readyPanel != null)
            {
                _readyPanel.SetActive(state == GameState.Ready);
            }

            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(state == GameState.GameOver);
            }

            if (_scoreText != null)
            {
                _scoreText.gameObject.SetActive(state == GameState.Playing);
            }

            if (state == GameState.GameOver)
            {
                ShowGameOver();
            }
        }

        private void HandleScoreChanged(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = score.ToString();
            }
        }

        private void ShowGameOver()
        {
            if (_finalScoreText != null)
            {
                _finalScoreText.text = $"Score: {GameManager.Instance.Score}";
            }

            if (_highScoreText != null)
            {
                _highScoreText.text = $"Best: {GameManager.Instance.HighScore}";
            }
        }
    }
}
