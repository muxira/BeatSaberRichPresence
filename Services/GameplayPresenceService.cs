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

        [InjectOptional] private readonly ILevelEndActions _levelEndActions;

        private int _currentCombo = 0;
        private float _currentEnergy = 1f;
        private long _startTimestamp = 0;
        private long _endTimestamp = 0;
        private bool _isFailed = false;

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
            
            if (_levelEndActions != null)
                _levelEndActions.levelFailedEvent += OnLevelFailed;

            _currentEnergy = _gameEnergyCounter.energy;
            _currentCombo = 0;
            _isFailed = false;

            _startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (long)_audioTimeSyncController.songTime;
            _endTimestamp = _startTimestamp + (long)_audioTimeSyncController.songLength;

            UpdateGameplayActivity(immediate: true);
        }

        public void Dispose()
        {
            if (_gameEnergyCounter != null)
                _gameEnergyCounter.gameEnergyDidChangeEvent -= OnEnergyChanged;
                
            if (_comboController != null)
                _comboController.comboDidChangeEvent -= OnComboChanged;
                
            if (_levelEndActions != null)
                _levelEndActions.levelFailedEvent -= OnLevelFailed;
        }

        public void Tick()
        {
        }

        private void OnEnergyChanged(float energy)
        {
            _currentEnergy = energy;
            if (!_isFailed) UpdateGameplayActivity();
        }

        private void OnComboChanged(int combo)
        {
            _currentCombo = combo;
            if (!_isFailed) UpdateGameplayActivity();
        }

        private void OnLevelFailed()
        {
            _isFailed = true;
            var level = _sceneSetupData.beatmapLevel;
            var difficulty = _sceneSetupData.beatmapKey.difficulty.ToString();
            
            var activity = new Activity
            {
                Details = $"Провал: {level.songName} - {level.songAuthorName}",
                State = $"Комбо: {_currentCombo}x",
                Assets = new ActivityAssets
                {
                    LargeImage = "default_icon", 
                    LargeText = difficulty
                }
            };
            
            _presenceManager.SetActivity(activity, immediate: true);
        }

        private void UpdateGameplayActivity(bool immediate = false)
        {
            if (_isFailed) return;
            
            var level = _sceneSetupData.beatmapLevel;
            var difficulty = _sceneSetupData.beatmapKey.difficulty.ToString();

            int energyPercent = Mathf.RoundToInt(_currentEnergy * 100);

            var activity = new Activity
            {
                Details = $"{level.songName} - {level.songAuthorName}",
                State = $"⚡ {energyPercent}% | Комбо: {_currentCombo}x",
                Timestamps = new ActivityTimestamps
                {
                    Start = _startTimestamp,
                    End = _endTimestamp
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
