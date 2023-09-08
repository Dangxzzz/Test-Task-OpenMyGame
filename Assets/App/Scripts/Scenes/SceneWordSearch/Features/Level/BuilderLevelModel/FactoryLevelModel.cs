using System.Linq;
using System.Collections.Generic;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;

            model.Words = value.words;
            
            model.InputChars = BuildListChars(value.words);

            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            List<char> uniqueChars = new List<char>();

            foreach (string word in words)
            {
                foreach (char letter in word)
                {
                    if (!uniqueChars.Contains(letter) || CountOfLetters(word, letter) > uniqueChars.Count(currentLetter => currentLetter == letter))
                    {
                        uniqueChars.Add(letter);
                    }
                }
            }

            return uniqueChars;
        }

        private int CountOfLetters(string word, char letter)
        {
            int count = 0;
            foreach (char currentLetter in word)
            {
                if (currentLetter == letter)
                {
                    count++;
                }
            }
            return count;
        }
    }
}