using UnityEngine;
using M3.Core.Domain;

namespace M3.Presentation.Gem
{
    public sealed class GemView : MonoBehaviour
    {
        public Position Position { get; private set; }

        public void Initialize(Position position, GemState state)
        {
            Position = position;
            UpdateVisual(state);
        }
        
        public void SetPosition(Position position)
        {
            Position = position;
        }
        
        public void UpdateVisual(GemState state)
        {
            var renderer = GetComponent<Renderer>();
            renderer.material.color = ColorFor(state.Color);

            // TODO: differentiate bombs visually
            if (state.Type == GemType.Bomb)
            {
                renderer.material.color *= 0.6f;
            }
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