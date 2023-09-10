using System;

namespace App.Scripts.Infrastructure.LevelSelection
{
    public class ServiceLevelSelection : IServiceLevelSelection
    {
        private readonly ConfigLevelSelection _configLevelSelection;

        private int _currentLevelIndex;
        private bool _isLevelCorrect=true;

        public ServiceLevelSelection(ConfigLevelSelection configLevelSelection)
        {
            _configLevelSelection = configLevelSelection;
            CurrentLevelIndex = configLevelSelection.InitLevelIndex;
        }
        
        
        public int CurrentLevelIndex
        {
            get => _currentLevelIndex;
            private set
            {
                _currentLevelIndex = value;
                OnSelectedLevelChanged?.Invoke();
            }
        }

        public event Action OnSelectedLevelChanged;
        
        public void UpdateSelectedLevel(int levelIndex)
        {
            if (levelIndex > _configLevelSelection.TotalLevelCount)
            {
                CurrentLevelIndex = 0;
                return;
            }

            if (levelIndex < 0)
            {
                CurrentLevelIndex = _configLevelSelection.TotalLevelCount;
                return;
            }

            CurrentLevelIndex = levelIndex;
        }

        public void IncrementCurrentLevelIndex()
        {
            _currentLevelIndex++;
        }

        public void DecrementCurrentLevelIndex()
        {
            _currentLevelIndex--;
        }
    }
}