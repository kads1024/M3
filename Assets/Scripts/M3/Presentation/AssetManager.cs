using AYellowpaper.SerializedCollections;
using M3.Core.Domain;
using UnityEngine;

namespace M3.Presentation
{
    /// <summary>
    /// Manages assets for referencing
    /// </summary>
    public class AssetManager : MonoBehaviour
    {
        [SerializedDictionary("GemColor", "GemSprite"), SerializeField] 
        private SerializedDictionary<GemColor,Sprite> _gemSprites;
        
        [SerializedDictionary("GemColor", "DestroyParticle"), SerializeField] 
        private SerializedDictionary<GemColor,ParticleSystem> _destroyEffects;

        public Sprite GetGemSprite(GemColor gemColor) { return _gemSprites[gemColor]; }
        
        public ParticleSystem GetDestroyEffect(GemColor gemColor) { return _destroyEffects[gemColor]; }
        
    }

}
