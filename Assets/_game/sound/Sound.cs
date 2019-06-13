using UnityEngine;
using Wakaman.Entities;
using Wakaman.Utilities;

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
            GameEvents.onCollect += OnCollectItem;
            GameEvents.onDeath += OnPlayerDeath;
            GameEvents.onExtraLife += OnExtraLife;
            GameEvents.onEatGhost += OnEatGhost;

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
                    audioMap?.GetEvent(AudioEvent.AudioEventType.PowerPellet)?.Play(audioSrc);
                    break;
                case CollectibleType.Fruit:
                    audioMap?.GetEvent(AudioEvent.AudioEventType.Fruit)?.Play(audioSrc);
                    break;
                default:
                    break;
            }
        }

        private void OnPlayerDeath()
        {
            this.Delay(Game.DEATH_STUTTER_TIME, () => {
                audioMap?.GetEvent(AudioEvent.AudioEventType.Death)?.Play(audioSrc);
            });
        }

        private void OnExtraLife()
        {
            audioMap?.GetEvent(AudioEvent.AudioEventType.ExtraCredit)?.Play(audioSrc);
        }

        private void OnEatGhost()
        {
            audioMap?.GetEvent(AudioEvent.AudioEventType.EatGhost)?.Play(audioSrc);
        }
    }
}
