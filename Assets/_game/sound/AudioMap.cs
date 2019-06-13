using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wakaman.Sound
{
    [CreateAssetMenu(menuName = "Wakaman/Audio/Audio map")]
    public class AudioMap : ScriptableObject
    {
        public List<AudioEvent> events;

        public AudioEvent GetEvent(AudioEvent.AudioEventType type)
        {
            foreach(var e in events)
            {
                if (e.Type == type)
                    return e;
            }
            Debug.LogErrorFormat("Non-configured AudioEvent in AudioMap \"{0}\" for event type \"{1}\"", name, type);
            return null;
        }
    }

    [System.Serializable]
    public class AudioEvent
    {
        public enum AudioEventType
        {
            Munch,
            PowerPellet,
            Fruit,
            Death,
            ExtraCredit,
            EatGhost
        }

        public enum SelectionMethod
        {
            Single,         // Plays a single clip, selects first.
            RoundRobin,     // Plays a single clip, selects via Round-robin.
            Random,         // Plays a single clip, selects random.
            Sequential      // Plays all the clips sequentially.
        }

        // Event meta data
        [SerializeField] private string name = "NEW_AUDIO_EVENT";
        [SerializeField] private AudioEventType type = AudioEventType.Munch;
        [SerializeField] private AudioClip[] clips;

        // Getters
        public AudioEventType Type { get => type; }

        // Clip play config
        [SerializeField] private SelectionMethod method = SelectionMethod.RoundRobin;
        [Range(0.0f, 1.0f)] public float volume = 1f;
        [Range(0.5f, 1.5f)] public float pitch = 1f;
        [Range(0.0f, 1.0f)] public float pitchVariance = 0f;

        // Runtime vars
        private int index = 0;

        // -------------------------- //
        // Playing
        // -------------------------- //

        public void Play(AudioSource src)
        {
            if (clips.Length == 0)
                return;

            src.volume = volume;
            src.pitch = Random.Range(pitch - (pitchVariance / 2f), pitch + (pitchVariance / 2f));
            if (method == SelectionMethod.Single)
            {
                src.PlayOneShot(clips[0]);
            }
            else if (method == SelectionMethod.RoundRobin)
            {
                src.PlayOneShot(clips[index >= clips.Length ? index = 0 : index]);
                index++;
            }
            else if (method == SelectionMethod.Random)
            {
                src.PlayOneShot(clips[Random.Range(0, clips.Length)]);
            }
            else if (method == SelectionMethod.Sequential)
            {
                // Creates additional sources if required.
                var sources = AddRequiredSources(src);
                // Schedules all clips sequentially in the multiple 
                // AudioSources.
                double accumulatedTime = 0.0;
                for (int i = 0; i < clips.Length; i++)
                {
                    sources[i].clip = clips[i];
                    sources[i].PlayScheduled(AudioSettings.dspTime + accumulatedTime);
                    accumulatedTime += clips[i].length;
                }
            }
        }

        // -------------------------- //
        // Helpers
        // -------------------------- //

        // AddRequiredSources: Creates an array of AudioSources to handle 
        //                     multiple clips scheduling, if necessary.
        private AudioSource[] AddRequiredSources(AudioSource src)
        {
            var sources = src.GetComponents<AudioSource>();
            if (sources.Length < clips.Length)
                for (int i = 0; i < clips.Length - sources.Length; i++)
                    src.gameObject.AddComponent<AudioSource>();
            sources = src.GetComponents<AudioSource>();
            for (int i = 0; i < sources.Length; i++)
            {
                sources[i].volume = src.volume;
                sources[i].pitch = src.pitch;
            }
            return sources;
        }
    }
}
