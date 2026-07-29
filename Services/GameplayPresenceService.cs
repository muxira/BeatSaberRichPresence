using System;
using UnityEngine;
using Zenject;
using Discord;

namespace DiscordRichPresence.Services
{
    public class GameplayPresenceService : IInitializable, IDisposable, ITickable
    {
        private readonly DiscordPresenceManager _presenceManager;
        private readonly AudioTimeSyncController _audioTimeSyncController;
        private readonly IGameEnergyCounter _gameEnergyCounter;
        private readonly IComboController _comboController;
        private readonly GameplayCoreSceneSetupData _sceneSetupData;

        private int _currentCombo = 0;
        private float _currentEnergy = 1f;

        public GameplayPresenceService(
            DiscordPresenceManager presenceManager,
            AudioTimeSyncController audioTimeSyncController,
            IGameEnergyCounter gameEnergyCounter,
            IComboController comboController,
            GameplayCoreSceneSetupData sceneSetupData)
        {
            _presenceManager = presenceManager;
            _audioTimeSyncController = audioTimeSyncController;
            _gameEnergyCounter = gameEnergyCounter;
            _comboController = comboController;
            _sceneSetupData = sceneSetupData;
        }

        public void Initialize()
        {
            // Hook events
            _gameEnergyCounter.gameEnergyDidChangeEvent += OnEnergyChanged;
            _comboController.comboDidChangeEvent += OnComboChanged;

            _currentEnergy = _gameEnergyCounter.energy;
            _currentCombo = 0;

            UpdateGameplayActivity(immediate: true);
        }

        public void Dispose()
        {
            if (_gameEnergyCounter != null)
                _gameEnergyCounter.gameEnergyDidChangeEvent -= OnEnergyChanged;
                
            if (_comboController != null)
                _comboController.comboDidChangeEvent -= OnComboChanged;
        }

        public void Tick()
        {
        }

        private void OnEnergyChanged(float energy)
        {
            _currentEnergy = energy;
            UpdateGameplayActivity();
        }

        private void OnComboChanged(int combo)
        {
            _currentCombo = combo;
            UpdateGameplayActivity();
        }

        private void UpdateGameplayActivity(bool immediate = false)
        {
            var level = _sceneSetupData.beatmapLevel;
            var difficulty = _sceneSetupData.beatmapKey.difficulty.ToString();
            
            // Format time for Discord native progress bar
            long startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (long)_audioTimeSyncController.songTime;
            long endTimestamp = startTimestamp + (long)_audioTimeSyncController.songLength;

            int energyPercent = Mathf.RoundToInt(_currentEnergy * 100);

            var activity = new Activity
            {
                Details = $"{level.songName} - {level.songAuthorName}",
                State = $"⚡ {energyPercent}% | Комбо: {_currentCombo}x",
                Timestamps = new ActivityTimestamps
                {
                    Start = startTimestamp,
                    End = endTimestamp
                },
                Assets = new ActivityAssets
                {
                    LargeImage = "default_icon", 
                    LargeText = difficulty
                }
            };

            _presenceManager.SetActivity(activity, immediate);
        }
    }
}
