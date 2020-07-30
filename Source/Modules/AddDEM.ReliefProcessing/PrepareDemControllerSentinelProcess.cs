using MilSpace.Configurations;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.Sentinel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.AddDem.ReliefProcessing
{
    internal class PrepareDemControllerSentinelProcess
    {
        IPrepareDemViewSentinelPeocess view;
        private IEnumerable<SentinelPairCoherence> currentPairs;

       

        public void SetView(IPrepareDemViewSentinelPeocess view)
        {
            this.view = view;
        }

        public IEnumerable<Tile> GetTilesFromDownloaded()
        {
            var demFacade = new DemPreparationFacade();
            return demFacade.GetAllSentinelProduct().Select(p => p.RelatedTile).Distinct().ToList();
        }
        public IEnumerable<SentinelPairCoherence> GetPairsFromDownloaded(string tile)
        {
            var demFacade = new DemPreparationFacade();
            var pathToSrc = MilSpaceConfiguration.DemStorages.SentinelDownloadStorage;
            var pairs = demFacade.GetPairsByTile(tile).Where(p => File.Exists(
               Path.Combine(pathToSrc, p.IdSceneBase + ".zip")) &&
               File.Exists(
               Path.Combine(pathToSrc, p.IdScentSlave + ".zip")));

            currentPairs = pairs;
            return pairs.ToList();
        }

        public SentinelPairCoherence GetPairPairBySceneName(string productName, bool isBase)
        {
            SentinelPairCoherence selectedPair = null;
            if (isBase)
            {
                selectedPair = currentPairs.FirstOrDefault(p => p.IdSceneBase == productName);
            }
            else { selectedPair = currentPairs.FirstOrDefault(p => p.IdScentSlave == productName); }

            return null;

        }

        public void CheckCoherence(SentinelPairCoherence selectedPair)
        {
            SantinelProcessing.EstimateCoherence(selectedPair);
        }
        public void PairProcessing(SentinelPairCoherence selectedPair)
        {
            SantinelProcessing.PairProcessing(selectedPair);
        }
    }
}
