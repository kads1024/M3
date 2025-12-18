using UnityEngine;
using System.Collections;
using M3.Core.Domain;

namespace M3.Presentation.Gem
{
    public sealed class GemView : MonoBehaviour
    {
        public Position Position { get; private set; }
        public GemState State { get; private set; }
        public void Initialize(Position position, GemState state)
        {
            Position = position;
            State = state;
            UpdateVisual(state);
        }

        public void SetLogicalPosition(Position position)
        {
            Position = position;
        }

        public void UpdateVisual(GemState state)
        {
            var renderer = GetComponent<Renderer>();
            renderer.material.color = ColorFor(state.Color);

            if (state.Type == GemType.Bomb)
                renderer.material.color *= 0.6f;
        }
        
        public IEnumerator AnimateMove(Vector3 target, float duration)
        {
            Vector3 start = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                transform.position = Vector3.Lerp(start, target, t);
                yield return null;
            }

            transform.position = target;
        }

        public IEnumerator AnimateScale(float from, float to, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float scale = Mathf.Lerp(from, to, t);
                transform.localScale = Vector3.one * scale;
                yield return null;
            }

            transform.localScale = Vector3.one * to;
        }

        
        private static Color ColorFor(GemColor color)
        {
            return color switch
            {
                GemColor.Red    => Color.red,
                GemColor.Blue   => Color.blue,
                GemColor.Green  => Color.green,
                GemColor.Yellow => Color.yellow,
                GemColor.Purple => Color.magenta,
                _ => Color.white
            };
        }
    }
}