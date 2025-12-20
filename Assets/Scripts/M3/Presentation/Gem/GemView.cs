using System;
using UnityEngine;
using System.Collections;
using M3.Core.Domain;
using M3.Utils;
using UnityEditor;

[Serializable] public class GemSpriteDictionary : SerializableDictionary<GemColor, Sprite> { }
[CustomPropertyDrawer(typeof(GemSpriteDictionary))] public class GemSpriteDictionaryDrawer : DictionaryDrawer<GemColor, Sprite> { }

namespace M3.Presentation.Gem
{
    public sealed class GemView : MonoBehaviour
    {
        [SerializeField] private GemSpriteDictionary _gemSprites;
        [SerializeField] private Sprite _bombSprite;
        
        public Position Position { get; private set; }
        public GemState State { get; private set; }
        
        private SpriteRenderer _renderer ;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(Position position, GemState state)
        {
            _renderer.color = Color.white;
            Position = position;
            State = state;
            UpdateVisual(state);
        }

        public void SetLogicalPosition(Position position)
        {
            Position = position;
        }

        private void UpdateVisual(GemState state)
        {
            _renderer.sprite = _gemSprites[state.Color];
            
            if (state.Type == GemType.Bomb)
            {
                _renderer.sprite = _bombSprite;
                _renderer.color = ColorFor(state.Color);
            }
                
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

        public void ResetView()
        {
            // Clear references
            State = null;
            Position = default;

            // Reset transform
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;
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