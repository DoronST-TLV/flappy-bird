using UnityEngine;

namespace FlappyBird
{
    /// <summary>
    /// The bird. Reads input in Update, applies motion in FixedUpdate, and
    /// reports collisions to the GameManager. Generates a placeholder sprite
    /// at runtime if none is assigned, so the game runs with zero assets.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Player : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _flapForce = 6.5f;
        [SerializeField] private float _gravityScale = 1.6f;
        [SerializeField] private float _maxTiltUp = 25f;
        [SerializeField] private float _maxTiltDown = -90f;
        [SerializeField] private float _tiltSpeed = 6f;

        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _startPosition;
        private bool _flapQueued;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _startPosition = transform.position;

            _rigidbody.gravityScale = _gravityScale;
            _rigidbody.freezeRotation = true;

            EnsurePlaceholderSprite();
            SetPhysicsFrozen(true);
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StateChanged += HandleStateChanged;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StateChanged -= HandleStateChanged;
            }
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            {
                return;
            }

            if (FlapPressed())
            {
                _flapQueued = true;
            }
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            {
                return;
            }

            if (_flapQueued)
            {
                _rigidbody.linearVelocity = new Vector2(0f, _flapForce);
                _flapQueued = false;
            }

            ApplyTilt();
        }

        private static bool FlapPressed()
        {
            return Input.GetKeyDown(KeyCode.Space)
                   || Input.GetMouseButtonDown(0)
                   || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        }

        private void ApplyTilt()
        {
            float velocityY = _rigidbody.linearVelocity.y;
            float targetAngle = velocityY >= 0f ? _maxTiltUp : _maxTiltDown;
            float t = Mathf.Clamp01(Mathf.Abs(velocityY) / _flapForce);
            float angle = Mathf.LerpAngle(0f, targetAngle, t);

            float smoothed = Mathf.LerpAngle(_rigidbody.rotation, angle, Time.fixedDeltaTime * _tiltSpeed);
            _rigidbody.MoveRotation(smoothed);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            {
                return;
            }

            if (other.CompareTag("Scorable"))
            {
                GameManager.Instance.AddScore(1);
                return;
            }

            GameManager.Instance.GameOver();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            {
                return;
            }

            GameManager.Instance.GameOver();
        }

        public void ResetPlayer()
        {
            transform.position = _startPosition;
            transform.rotation = Quaternion.identity;
            _rigidbody.rotation = 0f;
            _rigidbody.linearVelocity = Vector2.zero;
            _flapQueued = false;
            SetPhysicsFrozen(true);
        }

        private void HandleStateChanged(GameState state)
        {
            SetPhysicsFrozen(state != GameState.Playing);

            if (state == GameState.Playing)
            {
                // Give the run a gentle initial hop so the bird doesn't drop instantly.
                _rigidbody.linearVelocity = new Vector2(0f, _flapForce * 0.5f);
            }
        }

        private void SetPhysicsFrozen(bool frozen)
        {
            if (frozen)
            {
                _rigidbody.linearVelocity = Vector2.zero;
                _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            }
            else
            {
                _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        private void EnsurePlaceholderSprite()
        {
            if (_spriteRenderer.sprite != null)
            {
                return;
            }

            _spriteRenderer.sprite = PlaceholderSprites.SolidSquare(new Color(1f, 0.85f, 0.2f));
            _spriteRenderer.sortingOrder = 10;

            if (GetComponent<Collider2D>() == null)
            {
                CircleCollider2D circle = gameObject.AddComponent<CircleCollider2D>();
                circle.radius = 0.4f;
            }
        }
    }
}
