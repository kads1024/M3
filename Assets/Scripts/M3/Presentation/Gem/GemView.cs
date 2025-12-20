using UnityEngine;
using System.Collections;
using M3.Core.Domain;



namespace M3.Presentation.Gem
{
    public sealed class GemView : MonoBehaviour
    {
        [SerializeField] private Sprite _bombSprite;
        
        public Position Position { get; private set; }
        public GemState State { get; private set; }
        
        private SpriteRenderer _renderer ;

        private Sprite _sprite;
        private ParticleSystem _destroyEffect;
        
        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(Position position, GemState state, Sprite sprite, ParticleSystem destroyEffect)
        {
            Position = position;
            _sprite = sprite;
            _destroyEffect = destroyEffect;
            UpdateVisual(state);
        }

        public void SetLogicalPosition(Position position)
        {
            Position = position;
        }

        private void UpdateVisual(GemState state)
        {
            _renderer.sprite = _sprite;
            
            if (state.Type == GemType.Bomb)
            {
                _renderer.sprite = _bombSprite;
                _renderer.color = ColorFor(state.Color);
            }
            
            State = state;
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

        public IEnumerator ClearGem(float duration)
        {
            Instantiate(_destroyEffect, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(duration);
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

        public void ResetView()
        {
            // Clear references
            State = null;
            Position = default;

            // Reset transform
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;
            _sprite = null;
            _destroyEffect = null;
            _renderer.color = Color.white;
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