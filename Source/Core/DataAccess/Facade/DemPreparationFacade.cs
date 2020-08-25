using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    public class DemPreparationFacade
    {
        private static readonly Logger log = Logger.GetLoggerEx("DemPreparationFacade");
        public static SentinelSource GetSourceProductByName(string productName)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                var probuct = accessor.GetSoureceByName(productName);
                if (probuct == null)
                {
                    log.InfoEx($"Sentinel product {productName} was not found");
                    return null;
                }

                return probuct.Get();
            }
        }
        public SentinelSource AddSentinelSource(SentinelSource source)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                var newSource = accessor.AddSource(source.Get());

                if (newSource == null)
                {
                    log.ErrorEx($"There was an error on adding Sentinel source {source.SceneId}");
                    return null;
                }
                return newSource.Get();
            }
        }

        public IEnumerable<SentinelProduct> GetAllSentinelProduct()
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                try
                {
                    return accessor.GetAllS1SentinelProduct()?.Select(p => p.Get()).ToArray();
                }
                catch (Exception ex)
                {
                    log.ErrorEx(ex.Message);
                    return null;
                }
            }
        }

        public SentinelProduct GetSentinelProductByName(string productName)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                return accessor.GetSentinelProductByName(productName)?.Get();
            }
        }


        public string PathToDB
        {
            get
            {
                using (var accessor = new DemPreparationDataAccess())
                {
                    return accessor.PathToDB;
                }
            }
        }

        public SentinelProduct AddOrUpdateSentinelProduct(SentinelProduct product)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                var newProduct = accessor.AddOrUpdateProduct(product.Get());
                if (newProduct == null)
                {
                    log.ErrorEx($"There was an error on adding Sentinel source {product.Identifier}");
                    return null;
                }

                return newProduct.Get();
            }
        }

        public IEnumerable<SentinelPairCoherence> GetPairsByTile(string tile)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                return accessor.GetPairsByTile(tile).Select(p => p.Get()).ToList();
            }
        }


        public SentinelPairCoherence UpdateSentinelPairCoherence(SentinelPairCoherence pair)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                var pairRow = accessor.UpdatePair(pair.Get());
                if (pair == null)
                {
                    return null;
                    //Write logg
                }

                return pairRow.Get();
            }
        }

        public IEnumerable<SentinelProduct> GetProductsByTileName(string tileName)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                return accessor.GetSentinelProductsByTile(tileName)?.Select(p => p.Get()).ToArray();
            }
        }

        public SentinelPairCoherence GetSentinelPairCoherence(SentinelProduct product1, SentinelProduct product2)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                var pair = accessor.GetPairCoherence(product1.Identifier, product2.Identifier);

                if (pair == null)
                {
                    return null;
                    //Write logg
                }

                return pair.Get();
            }
        }

        public SentinelPairCoherence AddSentinelPairCoherence(SentinelProduct product1, SentinelProduct product2)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                var pair = accessor.AddPairCoherences(product1.Identifier, product2.Identifier);

                if (pair == null)
                {
                    return null;
                    //Write logg
                }

                return pair.Get();
            }
        }

        public SentinelTilesCoverage GetTileCoverage(string quaziTileName)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                return accessor.GetTileCoverage(quaziTileName)?.Get();
            }
        }

        public SentinelTilesCoverage AddOrUpdateTileCoverage(SentinelTilesCoverage qTile)
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                return accessor.AddOrUpdateTileCoverage(qTile.Get())?.Get();
            }
        }

        public IEnumerable<SentinelTilesCoverage> GetAllTilesCoverage()
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                return accessor.GetAllTileCoverages()?.Select(t => t.Get()).ToArray();
            }
        }
        public IEnumerable<SentinelTilesCoverage> GeTileCoveragesHaveGeometry()
        {
            using (var accessor = new DemPreparationDataAccess())
            {
                var tt = accessor.GetTileCoveragesHaveGeometry();
                return accessor.GetTileCoveragesHaveGeometry()?.Select(t => t.Get()).ToArray();
            }
        }

    }
}
