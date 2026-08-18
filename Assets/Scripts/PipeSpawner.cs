using System.Collections.Generic;
using UnityEngine;

namespace FlappyBird
{
    /// <summary>
    /// Spawns pipe pairs at a fixed interval while the game is Playing.
    /// If no prefab is assigned it builds a placeholder pair at runtime, so the
    /// game is playable without any imported assets. Assign a Pipe Sprite to
    /// give the placeholder pipes a real image instead of a plain square.
    /// </summary>
    public class PipeSpawner : MonoBehaviour
    {
        [Header("Spawning")]
        [SerializeField] private Pipe _pipePrefab;
        [SerializeField] private float _spawnInterval = 1.6f;
        [SerializeField] private float _spawnX = 10f;
        [SerializeField] private float _destroyX = -12f;
        [SerializeField] private float _scrollSpeed = 3.5f;

        [Header("Art")]
        [SerializeField] private Sprite _pipeSprite;

        [Header("Gap")]
        [SerializeField] private float _gapSize = 3.2f;
        [SerializeField] private float _minCenterY = -2.5f;
        [SerializeField] private float _maxCenterY = 2.5f;

        private float _timer;
        private readonly List<Pipe> _activePipes = new List<Pipe>();

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer >= _spawnInterval)
            {
                _timer = 0f;
                SpawnPipe();
            }
        }

        private void SpawnPipe()
        {
            float centerY = Random.Range(_minCenterY, _maxCenterY);
            var spawnPos = new Vector3(_spawnX, centerY, 0f);

            Pipe pipe = _pipePrefab != null
                ? Instantiate(_pipePrefab, spawnPos, Quaternion.identity)
                : BuildPlaceholderPipe(spawnPos);

            pipe.Init(_scrollSpeed, _destroyX);
            _activePipes.Add(pipe);
            _activePipes.RemoveAll(p => p == null);
        }

        public void ResetSpawner()
        {
            _timer = 0f;
            foreach (Pipe pipe in _activePipes)
            {
                if (pipe != null)
                {
                    Destroy(pipe.gameObject);
                }
            }

            _activePipes.Clear();
        }

        private Pipe BuildPlaceholderPipe(Vector3 position)
        {
            var root = new GameObject("Pipe");
            root.transform.position = position;
            Pipe pipe = root.AddComponent<Pipe>();

            const float pipeWidth = 1.4f;
            const float pipeHeight = 12f;
            var pipeColor = new Color(0.2f, 0.7f, 0.25f);
            float halfGap = _gapSize * 0.5f;

            CreatePipeBody(root.transform, "Top", new Vector3(0f, halfGap + pipeHeight * 0.5f, 0f),
                new Vector2(pipeWidth, pipeHeight), pipeColor);
            CreatePipeBody(root.transform, "Bottom", new Vector3(0f, -halfGap - pipeHeight * 0.5f, 0f),
                new Vector2(pipeWidth, pipeHeight), pipeColor);

            CreateScoreTrigger(root.transform, pipeWidth, _gapSize);

            return pipe;
        }

        private void CreatePipeBody(Transform parent, string name, Vector3 localPos, Vector2 size, Color color)
        {
            var body = new GameObject(name);
            body.transform.SetParent(parent, false);
            body.transform.localPosition = localPos;
            body.tag = "Obstacle";

            var renderer = body.AddComponent<SpriteRenderer>();
            renderer.sprite = _pipeSprite != null ? _pipeSprite : PlaceholderSprites.SolidSquare(color);
            renderer.sortingOrder = 5;
            body.transform.localScale = new Vector3(size.x, size.y, 1f);

            body.AddComponent<BoxCollider2D>();
        }

        private static void CreateScoreTrigger(Transform parent, float width, float gap)
        {
            var scoreZone = new GameObject("ScoreZone");
            scoreZone.transform.SetParent(parent, false);
            scoreZone.transform.localPosition = Vector3.zero;
            scoreZone.tag = "Scorable";

            var trigger = scoreZone.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            trigger.size = new Vector2(width * 0.25f, gap);
        }
    }
}