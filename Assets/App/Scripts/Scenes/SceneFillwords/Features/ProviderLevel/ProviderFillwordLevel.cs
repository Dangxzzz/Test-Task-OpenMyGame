using System.Collections.Generic;
using System.Linq;
using App.Scripts.Infrastructure.LevelSelection;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private TextAsset _levelsDataFile = Resources.Load<TextAsset>("Fillwords/pack_0");
        private TextAsset _dictionaryDataFile = Resources.Load<TextAsset>("Fillwords/words_list");
        private readonly IServiceLevelSelection _serviceLevelSelection;
        private string[] _dictionaryData;
        private List<string> _levelsData;
        private List<int> _invalidLevelsIndex=new();
        private ConfigLevelSelection _levelSelection;

        public ProviderFillwordLevel(ConfigLevelSelection configLevelSelection)
        {
            _levelSelection = configLevelSelection;
        }

        public ConfigLevelSelection GetLevelsCount()
        {
            return _levelSelection;
        }
        
        public GridFillWords LoadModel(int index)
        {
            _levelsData = _levelsDataFile.text.Split('\n').ToList();
            _dictionaryData = _dictionaryDataFile.text.Split('\n');
            CheckLevels();
            if (index-1 < 0 || index-1 >= _levelsData.Count)
            {
                Debug.LogError($"Invalid level index. {index}");
                return null;
            }
            string currentLevel = _levelsData[index-1];
            string[] levelParts = currentLevel.Trim().Split(' ');

            int gridSize = CalculateGridSize(levelParts.Where((_, i) => i % 2 != 0).ToArray());;

            if (gridSize > 0)
            {
                GridFillWords gridFillWords = CreateGridFillWords(gridSize);

                for (int i = 0; i < levelParts.Length; i += 2)
                {
                    int wordIndex = int.Parse(levelParts[i]);
                    string[] positions = levelParts[i + 1].Split(';');

                    FillGridWithLetters(gridFillWords, positions, wordIndex);

                }

                return gridFillWords;
            }
            else
            {
                _invalidLevelsIndex.Add(index-1);
                if (_levelsData.Count == 0)
                {
                    Debug.LogError("No valid levels!");
                }
                return null;
            }
        }

        private void CheckLevels()
        {
            if (_invalidLevelsIndex.Count > 0)
            {
                for (int i = 0; i < _levelsData.Count; i++)
                {
                    for (int j = 0; j < _invalidLevelsIndex.Count; j++)
                    {
                        DeleteLevel(i, j);
                    }
                }
            }
        }

        private void DeleteLevel(int i, int j)
        {
            if (i == _invalidLevelsIndex[j])
            {
                _levelsData.RemoveAt(i);
                _levelSelection.TotalLevelCount = _levelsData.Count;
            }
        }

        private int CalculateGridSize(string[] levelParts)
        {
            int gridSize = 0;

            foreach (string part in levelParts)
            {
                string[] positions = part.Split(';');
                gridSize += positions.Length;
            }

            int gridSizeRoot = (int)Mathf.Sqrt(gridSize);

            if (gridSizeRoot * gridSizeRoot == gridSize)
            {
                return gridSizeRoot;
            }
            else
            {
                return 0;
            }
        }


        private GridFillWords CreateGridFillWords(int gridSize)
        {
            return new GridFillWords(new Vector2Int(gridSize, gridSize));
        }

        private void FillGridWithLetters(GridFillWords gridFillWords, string[] positions, int wordIndex)
        {
            for (int j = 0; j < positions.Length; j++)
            {
                int position = int.Parse(positions[j]);

                int x = (position) % gridFillWords.Size.x;
                int y = (position) / gridFillWords.Size.x;

                if (x >= 0 && x < gridFillWords.Size.x && y >= 0 && y < gridFillWords.Size.y)
                {
                    SetLetter(gridFillWords, wordIndex, j, y, x);
                }
                else
                {
                    Debug.LogError($"Invalid size");
                }
            }
        }

        private void SetLetter(GridFillWords gridFillWords, int wordIndex, int j, int y, int x)
        {
            if (wordIndex >= 0 && wordIndex < _dictionaryData.Length)
            {
                string word = _dictionaryData[wordIndex].Trim();
                char charToAdd = word[j];
                CharGridModel charGridModel = new CharGridModel(charToAdd);
                gridFillWords.Set(y, x, charGridModel);
            }
            else
            {
                Debug.LogError("Invalid word index in level data.");
            }
        }
    }
}
