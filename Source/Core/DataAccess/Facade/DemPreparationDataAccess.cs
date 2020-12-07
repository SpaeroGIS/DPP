﻿using MilSpace.Configurations;
using MilSpace.DataAccess.Definition;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    internal class DemPreparationDataAccess : DataAccessor<DemPreparationContext>, IDisposable
    {
        private static string connectionStringTemplate = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30";
        private static string dbName = "DemPreparation.mdf";
        internal DemPreparationDataAccess()
        {
            log.InfoEx(
                $"Initialise DataAccess with connection: " +
                $"{ConnectionString}"
                );
        }

        public override string ConnectionString
        {
            get
            {
                var pathToDBFile = MilSpaceConfiguration.ConnectionProperty.DemPreparationDBConnection?.Trim();
                if (string.IsNullOrWhiteSpace(pathToDBFile))
                {

                    pathToDBFile = Path.Combine(MilSpaceConfiguration.DemStorages.SentinelStorageDBExternal, dbName);
                    if (!File.Exists(pathToDBFile))
                    {
                        log.ErrorEx($"Database file {pathToDBFile} does not exist.");
                        throw new FileNotFoundException(pathToDBFile);
                    }

                    return string.Format(connectionStringTemplate, pathToDBFile);
                }

                return pathToDBFile;

            }
        }

        public string PathToDB
        {
            get
            {
                System.Data.SqlClient.SqlConnectionStringBuilder cs = new System.Data.SqlClient.SqlConnectionStringBuilder(ConnectionString);
                return cs.AttachDBFilename;
            }
        }

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
                log.InfoEx($"UpdatePair. Pair {pair.idSceneBase} - {pair.idScentSlave} was successfully updated");
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
                log.WarnEx($"> UpdatePair EXCEPTION:{ex.Message}");
            }

            return null;
        }

        internal IEnumerable<S1PairCoherence> GetPairsByTile(string tile)
        {
            return 
                (from pair in context.S1PairCoherences
                 join prod in context.S1SentinelProducts on pair.idSceneBase equals prod.Identifier
                 where prod.TileName == tile
                 select pair).Distinct().Union(
                (from pair in context.S1PairCoherences
                 join prod in context.S1SentinelProducts on pair.idScentSlave equals prod.Identifier
                 where prod.TileName == tile
                 select pair)).Distinct();
        }

        internal S1PairCoherence GetPairCoherence(string source1, string source2)
        {
            return GetSourcePair(source1, source2);
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

        internal IEnumerable<S1SentinelProduct> GetSentinelProductsByTile(string tileName)
        {
            return GetAllS1SentinelProduct().Where(s => s.TileName.ToUpper() == tileName.ToUpper());
        }


        internal S1Sources AddSource(S1Sources source)
        {
            if (GetSoureceByName(source.idscene) == null)
            {
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

        internal S1SentinelProduct AddOrUpdateProduct(S1SentinelProduct product)
        {
            var productRec = context.S1SentinelProducts.FirstOrDefault(p => p.Identifier.ToUpper() == product.Identifier.ToUpper());

            try
            {
                if (productRec == null)
                {
                    product.Dto = DateTime.Now;
                    context.S1SentinelProducts.InsertOnSubmit(product);
                }
                else
                {
                    productRec.Dto = DateTime.Now;
                    productRec.Downloaded = product.Downloaded;
                }
                Submit();

                return context.S1SentinelProducts.FirstOrDefault(p => p.Identifier.ToUpper() == product.Identifier.ToUpper());

            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }

            return productRec;
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

        internal IEnumerable<S1TilesCoverage> GetAllTileCoverages()
        {
            return context.S1TilesCoverages;
        }
        internal IEnumerable<S1TilesCoverage> GetTileCoveragesHaveGeometry()
        {
            return context.S1TilesCoverages.Where( t => t.Wkt != null && t.Wkt.Length >= 8
            && t.DEMFilePath != null);
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

