using UnityEngine;

namespace FlappyBird
{
    /// <summary>
    /// The floor. Kills the bird on contact (via its collider + the Player's
    /// collision handler) and scrolls two tiles to fake infinite ground.
    /// Builds a placeholder strip at runtime if no sprite is assigned. Assign a
    /// Ground Sprite to give the floor a real image instead of a plain square.
    /// </summary>
    public class Ground : MonoBehaviour
    {
        [SerializeField] private float _scrollSpeed = 3.5f;
        [SerializeField] private float _tileWidth = 20f;
        [SerializeField] private bool _generatePlaceholder = true;

        [Header("Art")]
        [SerializeField] private Sprite _groundSprite;

        private Transform _tileA;
        private Transform _tileB;

        private void Awake()
        {
            if (_generatePlaceholder && transform.childCount == 0)
            {
                BuildPlaceholder();
            }

            if (transform.childCount >= 2)
            {
                _tileA = transform.GetChild(0);
                _tileB = transform.GetChild(1);
            }
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            {
                return;
            }

            if (_tileA == null || _tileB == null)
            {
                return;
            }

            float delta = _scrollSpeed * Time.deltaTime;
            _tileA.position += Vector3.left * delta;
            _tileB.position += Vector3.left * delta;

            RecycleTile(_tileA);
            RecycleTile(_tileB);
        }

        private void RecycleTile(Transform tile)
        {
            if (tile.position.x <= -_tileWidth)
            {
                float otherX = tile == _tileA ? _tileB.position.x : _tileA.position.x;
                tile.position = new Vector3(otherX + _tileWidth, tile.position.y, tile.position.z);
            }
        }

        private void BuildPlaceholder()
        {
            float y = transform.position.y;
            var color = new Color(0.85f, 0.7f, 0.35f);

            CreateTile("Ground_A", new Vector3(0f, y, 0f), color, true);
            CreateTile("Ground_B", new Vector3(_tileWidth, y, 0f), color, false);
        }

        private void CreateTile(string name, Vector3 worldPos, Color color, bool withCollider)
        {
            var tile = new GameObject(name);
            tile.transform.SetParent(transform, false);
            tile.transform.position = worldPos;

            var renderer = tile.AddComponent<SpriteRenderer>();
            renderer.sprite = _groundSprite != null ? _groundSprite : PlaceholderSprites.SolidSquare(color);
            renderer.sortingOrder = 8;
            tile.transform.localScale = new Vector3(_tileWidth, 2f, 1f);

            if (withCollider)
            {
                // One long collider on the parent covers the whole floor line.
                var box = gameObject.AddComponent<BoxCollider2D>();
                box.size = new Vector2(_tileWidth * 3f, 2f);
                box.offset = new Vector2(0f, worldPos.y);
                gameObject.tag = "Obstacle";
            }
        }
    }
}