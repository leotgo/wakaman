using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wakaman.Utilities;

namespace Wakaman.Sound
{
    public class BackgroundMusic : MonoBehaviour
    {
        // Definitions
        public enum BGMState
        {
            None,
            Siren,
            MatchStart,
            PowerPellet,
            Retreating,
            Death
        }

        // Clips
        [SerializeField] private AudioClip bgmMatchStart;
        [SerializeField] private AudioClip[] bgmSiren;
        [SerializeField] private AudioClip bgmPowerPellet;
        [SerializeField] private AudioClip bgmRetreating;

        // Clips play info
        [SerializeField] private float[] sirenStart;

        // Component Refs
        private AudioSource audioSrc;

        // Runtime
        private BGMState state;
        private int currSirenIndex;
        private int ghostsRetreating;

        private void Start()
        {
            audioSrc = GetComponent<AudioSource>();

            GameEvents.onMatchStart += OnMatchStart;
            GameEvents.onRoundStart += OnRoundStart;
            GameEvents.onDeath += OnDeath;
            GameEvents.onFinishLevel += OnFinishLevel;
            GameEvents.onCollect += OnCollect;
            GameEvents.onEatGhost += OnEatGhost;
            GameEvents.onStartPowerPellet += OnStartPowerPellet;
            GameEvents.onEndPowerPellet += OnEndPowerPellet;
            GameEvents.onGhostEndRetreat += OnGhostEndRetreat;

            state = BGMState.None;
        }

        private void OnMatchStart()
        {
            state = BGMState.MatchStart;
            currSirenIndex = 0;
            ghostsRetreating = 0;
            audioSrc.clip = bgmMatchStart;
            audioSrc.loop = false;
            audioSrc.Play();
        }

        private void OnStartPowerPellet()
        {
            if (state != BGMState.PowerPellet && state != BGMState.Retreating)
            {
                state = BGMState.PowerPellet;
                audioSrc.clip = bgmPowerPellet;
                audioSrc.loop = true;
                audioSrc.Play();
            }
        }

        private void OnEndPowerPellet()
        {
            state = BGMState.Siren;
            PlaySiren();
        }

        private void OnRoundStart()
        {
            currSirenIndex = 0;
            ghostsRetreating = 0;
            if(state != BGMState.MatchStart)
            {
                state = BGMState.None;
                audioSrc.Stop();
            }
            this.Delay(Game.RESPAWN_FREEZE_TIME, () => {
                state = BGMState.Siren;
                PlaySiren();
            });
        }

        private void OnDeath()
        {
            state = BGMState.None;
            audioSrc.Stop();
            if (!Game.IsGameOver)
            {
                this.Delay(Game.DEATH_STUTTER_TIME + Game.ROUND_START_TIME + Game.RESPAWN_FREEZE_TIME, () =>
                {
                    state = BGMState.Siren;
                    PlaySiren();
                });
            }
        }

        private void OnFinishLevel()
        {
            ghostsRetreating = 0;
            state = BGMState.None;
            audioSrc.loop = false;
            audioSrc.Stop();
        }

        private void OnCollect(CollectibleType type)
        {
            if (state == BGMState.PowerPellet || state == BGMState.Retreating)
                return;

            if (type == CollectibleType.Pellet)
            {
                int bestIndex = 0;
                for (int i = 0; i < bgmSiren.Length; i++)
                {
                    if (Game.ConsumedPelletsRatio > sirenStart[i])
                        bestIndex = i;
                }
                if(bestIndex != currSirenIndex)
                {
                    currSirenIndex = bestIndex;
                    PlaySiren();
                }
            }
        }

        private void OnEatGhost()
        {
            ghostsRetreating++;
            if(state != BGMState.Retreating)
            {
                this.Delay(Game.EAT_GHOST_STUTTER_TIME, () =>
                {
                    if (state != BGMState.None)
                    {
                        state = BGMState.Retreating;
                        audioSrc.clip = bgmRetreating;
                        audioSrc.loop = true;
                        audioSrc.Play();
                    }
                });
            }
        }

        private void OnGhostEndRetreat()
        {
            ghostsRetreating--;
            if(ghostsRetreating == 0)
            {
                if(Game.IsInPowerPelletMode)
                {
                    state = BGMState.PowerPellet;
                    audioSrc.clip = bgmPowerPellet;
                    audioSrc.loop = true;
                    audioSrc.Play();
                }
                else
                {
                    state = BGMState.Siren;
                    PlaySiren();
                }
            }
        }

        private void PlaySiren()
        {
            audioSrc.clip = bgmSiren[currSirenIndex];
            audioSrc.loop = true;
            audioSrc.Play();
        }
    }
}
