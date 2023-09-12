using System.Threading.Tasks;
using App.Scripts.Infrastructure.GameCore.States.SetupState;
using App.Scripts.Infrastructure.LevelSelection;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels.View.ViewGridLetters;
using App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.States.Setup
{
    public class HandlerSetupFillwords : IHandlerSetupLevel
    {
        private readonly ContainerGrid _containerGrid;
        private readonly IProviderFillwordLevel _providerFillwordLevel;
        private readonly IServiceLevelSelection _serviceLevelSelection;
        private readonly ViewGridLetters _viewGridLetters;
        private readonly ConfigLevelSelection _configLevelSelection;

        public HandlerSetupFillwords(IProviderFillwordLevel providerFillwordLevel,
            IServiceLevelSelection serviceLevelSelection,
            ViewGridLetters viewGridLetters, ContainerGrid containerGrid)
        {
            _providerFillwordLevel = providerFillwordLevel;
            _serviceLevelSelection = serviceLevelSelection;
            _viewGridLetters = viewGridLetters;
            _containerGrid = containerGrid;
        }

        public Task Process()
        {
            var model = _providerFillwordLevel.LoadModel(_serviceLevelSelection.CurrentLevelIndex);
            if (model == null)
            {
                for (int i = 0; i < _providerFillwordLevel.GetLevelsCount().TotalLevelCount; i++)
                {
                    model = _providerFillwordLevel.LoadModel(_serviceLevelSelection.CurrentLevelIndex);
                    if (model != null)
                    {
                        break;
                    }
                }
            }
            _viewGridLetters.UpdateItems(model);
            _containerGrid.SetupGrid(model, _serviceLevelSelection.CurrentLevelIndex);
            return Task.CompletedTask;
        }
    }
}