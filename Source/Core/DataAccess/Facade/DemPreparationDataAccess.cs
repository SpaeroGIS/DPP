using MilSpace.Configurations;
using MilSpace.DataAccess.Definition;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    internal class DemPreparationDataAccess : DataAccessor<DemPreparationContext>, IDisposable
    {
        internal DemPreparationDataAccess()
        {
            log.InfoEx(
                $"Initialise GeoCalculatorDataAccess with connection: " +
                $"{MilSpaceConfiguration.ConnectionProperty.DemPreparationDBConnection}"
                );
        }

        public override string ConnectionString => MilSpaceConfiguration.ConnectionProperty.DemPreparationDBConnection;

        internal S1Sources GetSoureceByName(string sourceName)
        {
            try
            {
                return context.S1Sources.FirstOrDefault(s => s.idscene.ToUpper().Equals(sourceName.ToUpper()));
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }
            return null;
        }

        internal S1PairCoherence GetSourcePair(string source1, string source2)
        {
            return context.S1PairCoherences.FirstOrDefault(p => (p.idSceneBase == source1 && p.idScentSlave == source2) || (p.idSceneBase == source2 && p.idScentSlave == source1));
        }

        internal S1PairCoherence GetPair(S1PairCoherence pair)
        {
            return context.S1PairCoherences.FirstOrDefault(p => (p.idrow == pair.idrow));
        }

        internal S1PairCoherence UpdatePair(S1PairCoherence pair)
        {
            try
            {
                var pairToUpdate = context.S1PairCoherences.FirstOrDefault(p => (p.idrow == pair.idrow));

                pairToUpdate.fdeviation = pair.fdeviation;
                pairToUpdate.fmax = pair.fmax;
                pairToUpdate.fmin = pair.fmin;
                pairToUpdate.fmean = pair.fmean;

                Submit();
                log.InfoEx($"Pair {pair.idSceneBase} - {pair.idScentSlave} was successfully updated");
                return context.S1PairCoherences.FirstOrDefault(p => (p.idrow == pair.idrow));
            }
            catch (MilSpaceDataException ex)
            {
                log.WarnEx(ex.Message);

                if (ex.InnerException != null)
                {
                    log.WarnEx(ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return null; ;
        }

        internal IEnumerable<S1PairCoherence> GetPairsByTile(string tile)
        {

            return (from pair in context.S1PairCoherences
                    join prod in context.S1SentinelProducts on
                 pair.idSceneBase equals prod.Identifier
                    where prod.TileName == tile
                    select pair).Distinct().Union(
                 (from pair in context.S1PairCoherences
                  join prod in context.S1SentinelProducts on
                  pair.idScentSlave equals prod.Identifier
                  where prod.TileName == tile
                  select pair)
                  ).Distinct();

        }


        internal S1PairCoherence AddPairCoherences(string source1, string source2)
        {
            var pair = GetSourcePair(source1, source2);
            if (pair != null)
            {
                return pair;
            }

            pair = new S1PairCoherence();

            pair.idSceneBase = source1;
            pair.idScentSlave = source2;
            pair.soper = Environment.UserName;
            pair.dto = DateTime.Now;

            try
            {
                context.S1PairCoherences.InsertOnSubmit(pair);
                Submit();

                return GetSourcePair(pair.idSceneBase, pair.idScentSlave);

            }
            catch (MilSpaceDataException ex)
            {
                log.WarnEx(ex.Message);

                if (ex.InnerException != null)
                {
                    log.WarnEx(ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");

            }

            return null;
        }

        internal S1Sources AddSource(S1Sources source)
        {
            if (GetSoureceByName(source.idscene) == null)
            {
                context.S1Sources.InsertOnSubmit(source);
                try
                {
                    context.S1Sources.InsertOnSubmit(source);
                    Submit();

                    return GetSoureceByName(source.idscene);

                }
                catch (MilSpaceDataException ex)
                {
                    log.WarnEx(ex.Message);

                    if (ex.InnerException != null)
                    {
                        log.WarnEx(ex.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    log.WarnEx($"Unexpected exception:{ex.Message}");

                }
            }

            return null;
        }

        internal S1SentinelProduct AddProduct(S1SentinelProduct product)
        {
            var productRec = context.S1SentinelProducts.FirstOrDefault(p => p.Identifier.ToUpper() == product.Identifier.ToUpper());

            if (productRec == null)
            {
                try
                {
                    product.Dto = DateTime.Now;
                    context.S1SentinelProducts.InsertOnSubmit(product);
                    Submit();
                    return context.S1SentinelProducts.FirstOrDefault(p => p.Identifier.ToUpper() == product.Identifier.ToUpper());
                }
                catch (Exception ex)
                {
                    log.WarnEx($"Unexpected exception:{ex.Message}");
                }
            }

            return null;
        }

        internal IEnumerable<S1SentinelProduct> GetAllS1SentinelProduct()
        {
            return context.S1SentinelProducts;
        }

        internal S1SentinelProduct GetSentinelProductByName(string identifier)
        {
            try
            {
                return context.S1SentinelProducts.FirstOrDefault(p => p.Identifier == identifier);
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }
            return null;
        }

        internal S1TilesCoverage GetTileCoverage(string quaziTileName)
        {
            return context.S1TilesCoverages.FirstOrDefault(p => p.QuaziTileName.ToUpper() == quaziTileName.ToUpper());
        }

        internal IEnumerable<S1TilesCoverage> GetAllTileCoverage()
        {
            return context.S1TilesCoverages;
        }


        internal S1TilesCoverage AddOrUpdateTileCoverage(S1TilesCoverage tileCover)
        {
            var tileCoverRecord = context.S1TilesCoverages.FirstOrDefault(p => p.QuaziTileName.ToUpper() == tileCover.QuaziTileName.ToUpper());
            try
            {
                if (tileCoverRecord == null)
                {
                    tileCover.Dto = DateTime.Now;
                    tileCover.sOper = Environment.UserName;
                    context.S1TilesCoverages.InsertOnSubmit(tileCover);
                }
                else
                {
                    tileCoverRecord.Status = tileCover.Status;
                    tileCoverRecord.DEMFilePath = tileCover.DEMFilePath;
                    tileCoverRecord.Wkt = tileCover.Wkt;
                    tileCoverRecord.Dto = DateTime.Now;
                }
                Submit();
                return context.S1TilesCoverages.FirstOrDefault(p => p.QuaziTileName.ToUpper() == tileCover.QuaziTileName.ToUpper());
            }
            catch (Exception ex)
            {
                log.WarnEx($"AddOrUpdateTileCoverage: Unexpected exception:{ex.Message}");
            }

            return null;
        }
    }
}

