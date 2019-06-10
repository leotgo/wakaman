using UnityEngine;
using Wakaman.Entities;

namespace Wakaman.Sound
{
    // Sound: 
    // - Handles detection and playing of sound events.
    [RequireComponent(typeof(AudioSource))]
    public class Sound : MonoBehaviour
    {
        // Component Refs
        private AudioSource audioSrc;

        // Audio Settings
        [SerializeField] private AudioMap audioMap;

        // -------------------------- //
        // Monobehaviour
        // -------------------------- //

        private void Start()
        {
            audioSrc = GetComponent<AudioSource>();
            Game.onCollect += OnCollectItem;
            Game.onDeath += OnPlayerDeath;

            if (!audioMap)
                Debug.LogWarningFormat("AudioMap is not set in {0}", name);
        }

        // -------------------------- //
        // Sound Events
        // -------------------------- //

        private void OnCollectItem(CollectibleType type)
        {
            switch (type)
            {
                case CollectibleType.Pellet:
                    audioMap?.GetEvent(AudioEvent.AudioEventType.Munch)?.Play(audioSrc);
                    break;
                case CollectibleType.PowerPellet:
                    break;
                case CollectibleType.Fruit:
                    break;
                default:
                    break;
            }
        }

        private void OnPlayerDeath()
        {
            audioMap?.GetEvent(AudioEvent.AudioEventType.Death)?.Play(audioSrc);
        }
    }
}
