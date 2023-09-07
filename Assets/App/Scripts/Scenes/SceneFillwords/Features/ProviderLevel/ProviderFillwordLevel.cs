using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private TextAsset _levelsDataFile = Resources.Load<TextAsset>("Fillwords/pack_0");
        private TextAsset _dictionaryDataFile = Resources.Load<TextAsset>("Fillwords/words_list");
        private string[] _dictionaryData;
        private string[] _levelsData;

        public GridFillWords LoadModel(int index)
        {
            Debug.Log($"Try to load level : {index}");
            _levelsData = _levelsDataFile.text.Split('\n');
            Debug.Log($"Levels count: {_levelsData.Length}");
            _dictionaryData = _dictionaryDataFile.text.Split('\n');

            if (index < 0 || index >= _levelsData.Length)
            {
                Debug.LogError("Invalid level index.");
                return null;
            }

            string currentLevel = _levelsData[index];
            // Debug.Log($"CurrentLevel {currentLevel}");
            string[] levelParts = currentLevel.Trim().Split(' ');

            int gridSize = CalculateGridSize(levelParts.Where((_, i) => i % 2 != 0).ToArray());;
            // Debug.Log($"GridSize: {gridSize} ");

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
                return LoadModel(index + 2);
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
                    if (wordIndex >= 0 && wordIndex < _dictionaryData.Length)
                    {
                        string word = _dictionaryData[wordIndex].Trim();
                        // Debug.Log($"Letter: {word[j]}");
                        char charToAdd = word[j];
                        CharGridModel charGridModel = new CharGridModel(charToAdd);
                        gridFillWords.Set(y, x, charGridModel);
                        Debug.LogError($"Y: {y} X: {x} Char{charGridModel}");
                    }
                    else
                    {
                        Debug.LogError("Invalid word index in level data.");
                    }
                }
                else
                {
                    Debug.LogError($"Invalid size");
                }
            }
        }
    }
}
