using System.Collections.Generic;
using UnityEngine;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {

        private LevelInfo _levelInfo = new();
        public LevelInfo LoadLevelData(int levelIndex)
        {
            string jsonFileName = "WordSearch/Levels/" + levelIndex;
            TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);

            // LevelInfo _levelInfo = new LevelInfo();

            if (jsonFile != null)
            {
                
                    LevelData levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
                    if (levelData != null)
                    {
                        _levelInfo.words = new List<string>(levelData.words);
                    }
                    else
                    {
                        Debug.LogError("Error loading level data");
                    }
            }
            else
            {
                Debug.LogError("Level file not found: " + jsonFileName);
            }

            return _levelInfo;
        }
    }
    
    [System.Serializable]
    public class LevelData
    {
        public string[] words;
    }
}