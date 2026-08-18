using UnityEngine;

namespace FlappyBird
{
    /// <summary>
    /// Generates simple solid-color sprites at runtime. This lets the whole
    /// game run without any imported art. Swap real sprites into the
    /// SpriteRenderer / [SerializeField] slots to replace these.
    /// </summary>
    public static class PlaceholderSprites
    {
        private static Sprite _cachedSquare;

        /// <summary>
        /// A 1x1 world-unit white square, tinted via the SpriteRenderer color.
        /// Reused across callers; tint through the renderer, not the sprite.
        /// </summary>
        public static Sprite SolidSquare(Color color)
        {
            var texture = new Texture2D(4, 4, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color[16];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return Sprite.Create(
                texture,
                new Rect(0, 0, 4, 4),
                new Vector2(0.5f, 0.5f),
                4f);
        }
    }
}
