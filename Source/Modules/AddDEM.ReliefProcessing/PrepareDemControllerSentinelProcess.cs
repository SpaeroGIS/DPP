using MilSpace.Configurations;
using MilSpace.Core.Actions;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.Sentinel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MilSpace.AddDem.ReliefProcessing
{
    internal class PrepareDemControllerSentinelProcess
    {
        IPrepareDemViewSentinelPeocess view;
        private IEnumerable<SentinelProduct> currentPproducts;
        public event ActionProcessCommandLineDelegate OnErrorProcessing;
        public event ActionProcessCommandLineDelegate OnProcessing;

        public void SetView(IPrepareDemViewSentinelPeocess view)
        {
            this.view = view;
        }

        public IEnumerable<Tile> GetTilesFromDownloaded()
        {
            var demFacade = new DemPreparationFacade();
            return demFacade.GetAllSentinelProduct()?.Select(p => p.RelatedTile).Distinct().ToList();
        }

        public IEnumerable<SentinelProduct> GetProductsByTileName(string tile)
        {
            var demFacade = new DemPreparationFacade();
            var pathToSrc = MilSpaceConfiguration.DemStorages.SentinelDownloadStorageExternal;
            currentPproducts = demFacade.GetProductsByTileName(tile).Where(
                p => File.Exists(Path.Combine(pathToSrc, p.Identifier + ".zip")));

            return currentPproducts.ToArray();
        }

        public IEnumerable<SentinelProduct> GetPairByProduct()
        {
            var baseScene = view.SelectedProductDem;
            if (baseScene == null)
            {
                return new SentinelProduct[0];
            }

            var slaveScene = currentPproducts.Where(p => !p.Equals(baseScene) &&
                p.OrbitNumber == baseScene.OrbitNumber &&
                12 - Math.Abs((p.DateTime - baseScene.DateTime).TotalDays) < 0.05 &&
                p.ExtendEqual(baseScene)).FirstOrDefault();

            var result = new List<SentinelProduct> { baseScene };

            if (slaveScene == null)
            {
                return result;
            }

            var facade = new DemPreparationFacade();

            view.SentinelPairDem = facade.GetSentinelPairCoherence(baseScene, slaveScene);

            result.Add(slaveScene);

            return result;
        }

        public void ClearSelected()
        {
            currentPproducts = null;
            view.SentinelPairDem = null;
        }

        public SentinelProduct GetSentinelProductByIdentifier(string productName)
        {
            return currentPproducts?.FirstOrDefault(p => p.Identifier == productName);
        }

        SantinelProcessing processing;

        private SantinelProcessing Processing
        {
            get
            {
                if (processing == null)
                {
                    processing = new SantinelProcessing();
                    processing.OnProcessing = OnProcessing;
                    processing.OnErrorProcessing = OnErrorProcessing;
                }

                return processing;
            }
        }


        public void CheckCoherence()
        {
            CheckPairExistance();
            Processing.EstimateCoherence(view.SentinelPairDem);

        }
        public void PairProcessing()
        {
            CheckPairExistance();
            Processing.PairProcessing(view.SentinelPairDem, view.QuaziTilesDefinition);
        }

        private void CheckPairExistance()
        {
            if (view.SentinelPairDem == null)
            {
                var scenes = GetPairByProduct();
                if (scenes.Count() == 2)
                {
                    view.SentinelPairDem = AddSentinelPairCoherence(scenes.First(), scenes.Last());
                }
            }
        }

        public SentinelPairCoherence AddSentinelPairCoherence(SentinelProduct product1, SentinelProduct product2)
        {
            var demFacade = new DemPreparationFacade();
            demFacade.AddOrUpdateSentinelProduct(product1);
            demFacade.AddOrUpdateSentinelProduct(product2);
            view.SentinelPairDem = demFacade.AddSentinelPairCoherence(product1, product2);
            return view.SentinelPairDem;
        }
    }
}
