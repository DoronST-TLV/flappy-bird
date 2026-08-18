using UnityEngine;

namespace FlappyBird
{
    /// <summary>
    /// A single pipe pair. Scrolls left every frame and destroys itself once
    /// fully off-screen. Movement pauses when the game is not in Playing state.
    /// </summary>
    public class Pipe : MonoBehaviour
    {
        private float _speed;
        private float _destroyX;

        public void Init(float speed, float destroyX)
        {
            _speed = speed;
            _destroyX = destroyX;
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            {
                return;
            }

            transform.position += Vector3.left * (_speed * Time.deltaTime);

            if (transform.position.x < _destroyX)
            {
                Destroy(gameObject);
            }
        }
    }
}
