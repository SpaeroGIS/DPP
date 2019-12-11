using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Visibility
{
    public static class ArcMapHelper
    {


        //    { VisibilityCalculationResultsEnum.VisibilityObservStationClip , "_imgc"},
        //    { VisibilityCalculationResultsEnum.VisibilityObservStationClipSingle , "_imgcs"},
        //    { VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly , "_imgt"},
        //    { VisibilityCalculationResultsEnum.VisibilityAreaTrimmedByPolySingle , "_imgts"},

        internal static Dictionary<VisibilityCalculationResultsEnum, bool> LayersSequence = new Dictionary<VisibilityCalculationResultsEnum, bool>

        {
            { VisibilityCalculationResultsEnum.ObservationPoints, true},
            { VisibilityCalculationResultsEnum.VisibilityAreasPotential, true},
            { VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle, true},
            { VisibilityCalculationResultsEnum.ObservationObjects, false},
            { VisibilityCalculationResultsEnum.VisibilityAreaPolygons, true},
            { VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle, false},
            { VisibilityCalculationResultsEnum.VisibilityAreaRaster, false},
            { VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle , false},
            { VisibilityCalculationResultsEnum.VisibilityObservStationClip , false},
            { VisibilityCalculationResultsEnum.VisibilityObservStationClipSingle , false},
            { VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly , false},
            { VisibilityCalculationResultsEnum.VisibilityAreaTrimmedByPolySingle , false}
        };

        static Logger logger = Logger.GetLoggerEx("MilSpace.Visibility.ArcMapHelper");

        private static Dictionary<VisibilityCalculationResultsEnum, Action<ILayer, IColor, short>> mapResultAction = 
            new Dictionary<VisibilityCalculationResultsEnum, Action<ILayer, IColor, short>>
        {
            { VisibilityCalculationResultsEnum.VisibilityAreasPotential, (layer, fillColor, transparency) => {

                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                    simpleFillSymbol.Color = new RgbColor()
                    {
                        Transparency = 0
                    };

                    ICartographicLineSymbol outline = new CartographicLineSymbol
                    {
                        Width = 2,
                        Color = new RgbColor()
                        {
                            Red = 100,
                            Green = 100,
                            Blue = 100
                        }
                    };

                    simpleFillSymbol.Outline = outline;
                    EsriTools.SetFeatureLayerStyle(layer as IFeatureLayer,  simpleFillSymbol as ISymbol);
                }
            },
            { VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle, (layer, fillColor, transparency) => {

                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                    simpleFillSymbol.Color = new RgbColor()
                    {
                        Transparency = 0
                    };

                    ICartographicLineSymbol outline = new CartographicLineSymbol
                    {
                        Width = 0.4,
                        Color = new RgbColor()
                        {
                            Red = 100,
                            Green = 100,
                            Blue = 100
                        }
                    };

                    simpleFillSymbol.Outline = outline;
                    EsriTools.SetFeatureLayerStyle(layer as IFeatureLayer,  simpleFillSymbol as ISymbol);
                }
            },
            { VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly, (layer, fillColor, transparency) => {

                    if (layer is IRasterLayer rasterLayer )
                    {
                        logger.InfoEx($"Setting unique values for remdering \"{rasterLayer.Name}\" ");
                        try
                        {
                            var render = EsriTools.GetCalclResultRender(rasterLayer.Raster, "Value");
                            if (render == null)
                            {
                                logger.ErrorEx($"The raster \"{rasterLayer.Name}\" doesn't have a table");
                            }

                            rasterLayer.Renderer = render;
                            logger.InfoEx($"Unique values for remdering \"{rasterLayer.Name}\" was set");
                        }
                        catch (KeyNotFoundException ex)
                        {
                            logger.ErrorEx($"The field \"Value\" was not found in the raster table");
                            logger.ErrorEx(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorEx(ex.Message);
                        }
                    }
                }
            },
            { VisibilityCalculationResultsEnum.VisibilityObservStationClip, (layer, fillColor, transparency) => {

                    if (layer is IRasterLayer rasterLayer )
                    {
                         logger.InfoEx($"Setting unique values for remdering \"{rasterLayer.Name}\" ");
                        try
                        {
                            var render = EsriTools.GetCalclResultRender(rasterLayer.Raster, "Value");
                            if (render == null)
                            {
                                logger.ErrorEx($"The raster \"{rasterLayer.Name}\" doesn't have a table");
                            }

                            rasterLayer.Renderer = render;
                            logger.InfoEx($"Unique values for remdering \"{rasterLayer.Name}\" was set");
                        }
                        catch (KeyNotFoundException ex)
                        {
                            logger.ErrorEx($"The field \"Value\" was not found in the raster table");
                            logger.ErrorEx(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorEx(ex.Message);
                        }
                    }
                }
            },
            { VisibilityCalculationResultsEnum.VisibilityAreaRaster, (layer, fillColor, transparency) => {

                  if (layer is IRasterLayer rasterLayer )
                    {
                        logger.InfoEx($"Setting unique values for remdering \"{rasterLayer.Name}\" ");
                        try
                        {
                            var render = EsriTools.GetCalclResultRender(rasterLayer.Raster, "Value");
                            if (render == null)
                            {
                                logger.ErrorEx($"The raster \"{rasterLayer.Name}\" doesn't have a table");
                            }

                            rasterLayer.Renderer = render;
                            logger.InfoEx($"Unique values for remdering \"{rasterLayer.Name}\" was set");
                        }
                        catch (KeyNotFoundException ex)
                        {
                            logger.ErrorEx($"The field \"Value\" was not found in the raster table");
                            logger.ErrorEx(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorEx(ex.Message);
                        }
                    }
                }
            },
            { VisibilityCalculationResultsEnum.VisibilityAreaPolygons, (layer, fillColor, transparency) => {

                    if (layer is IFeatureLayer polygonLayer )
                    {

                        string valuesField = "gridcode";

                        var attrTable = polygonLayer.FeatureClass as ITable;
                        var fld = attrTable.FindField(valuesField);

                        var filter = new QueryFilter();
                        filter.SubFields = valuesField;
                        IQueryFilterDefinition2 filterDefinition = (IQueryFilterDefinition2)filter;

                        filterDefinition.PostfixClause = $"ORDER BY {valuesField}";
                        filterDefinition.PrefixClause = $"DISTINCT  {valuesField}";
                        var uniwuevaluesr = attrTable.Search(filter, false);

                        var row =uniwuevaluesr.NextRow();
                        List<int> ids = new List<int>();
                        
                        while (row != null)
                        {
                            ids.Add((int)row.Value[fld]);
                            row =uniwuevaluesr.NextRow();
                        }


                        IAlgorithmicColorRamp algColorRamp = new AlgorithmicColorRampClass();

                        //Create the start and end colors
                        IRgbColor startColor = new RgbColor()
                        {
                            Red = 255,
                            Green = 255,
                            Blue = 115
                        };
                        IRgbColor endColor = new RgbColor()
                        {
                            Red = 115,
                            Green = 38,
                            Blue = 0

                        };
                           
                        //Set the Start and End Colors
                        algColorRamp.ToColor = endColor;
                        algColorRamp.FromColor = startColor;

                        //Set the ramping Alglorithm 
                        algColorRamp.Algorithm = esriColorRampAlgorithm.esriCIELabAlgorithm;

                        //Set the size of the ramp (the number of colors to be derived)
                        algColorRamp.Size = ids.Count;


                        //Create the ramp
                        bool ok = true;
                        algColorRamp.CreateRamp(out ok);

                        if (ok)
                        {
                            IUniqueValueRenderer uniqueRen = new UniqueValueRenderer();
                            uniqueRen.FieldCount = 1;
                            uniqueRen.Field[0]=valuesField;


                            var fldIndex = uniwuevaluesr.FindField(valuesField);
                            if (fldIndex < 0)
                            {
                                throw new KeyNotFoundException(valuesField);
                            }

                            int valueClass = 0;
                        
                            foreach(var uniqueValue in ids)
                            {
                                var classValue = uniqueValue;
                                ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbol();
                                simpleFillSymbol.Color = algColorRamp.Color[valueClass++];
                                simpleFillSymbol.Outline = new CartographicLineSymbol
                                {
                                    Width = 0.4,
                                    Color = new RgbColor()
                                    {
                                        Red = 100,
                                        Green = 100,
                                        Blue = 100
                                    }
                                };

                                uniqueRen.AddValue($"{classValue}", string.Empty, simpleFillSymbol as ISymbol);
                                uniqueRen.Label[$"{classValue}"] = $"{classValue}";
                                uniqueRen.Symbol[$"{classValue}"] = simpleFillSymbol as ISymbol;
                            //, $"{classValue}", simpleFillSymbol as ISymbol);
                            }
                            IGeoFeatureLayer geoFeatureLayer = (IGeoFeatureLayer)polygonLayer;
                            geoFeatureLayer.Renderer = (IFeatureRenderer)uniqueRen;
                        }
                    }
                }
            },
            { VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle, (layer, fillColor, transparency) => {

                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                    simpleFillSymbol.Color = new RgbColor()
                    {
                        Transparency = 33,
                        Red = 255,
                        Green = 255,
                        Blue = 115
                    };

                    ICartographicLineSymbol outline = new CartographicLineSymbol
                    {
                        Width = 0.4,
                        Color = new RgbColor()
                        {
                            Red = 100,
                            Green = 100,
                            Blue = 100
                        }
                    };

                    simpleFillSymbol.Outline = outline;
                    EsriTools.SetFeatureLayerStyle(layer as IFeatureLayer,  simpleFillSymbol as ISymbol);
                }
            }
        };

        private static Dictionary<VisibilityCalculationResultsEnum, Func<ILayer, ISymbol>> mapResultSympols = new Dictionary<VisibilityCalculationResultsEnum, Func<ILayer, ISymbol>>
        {
            { VisibilityCalculationResultsEnum.VisibilityAreasPotential, (layer) => {

                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                    simpleFillSymbol.Color = new RgbColor()
                    {
                        Transparency = 0
                    };

                    ICartographicLineSymbol outline = new CartographicLineSymbol
                    {
                        Width = 2,
                        Color = new RgbColor()
                        {
                            Red = 100,
                            Green = 100,
                            Blue = 100
                        }
                    };

                    simpleFillSymbol.Outline = outline;
                    return (ISymbol)simpleFillSymbol;
                }
            },
            { VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle, (layer) => {

                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();



                    simpleFillSymbol.Color = new RgbColor()
                    {
                        Transparency = 0
                    };

                    ICartographicLineSymbol outline = new CartographicLineSymbol
                    {
                        Width = 0.4,
                        Color = new RgbColor()
                        {
                            Red = 100,
                            Green = 100,
                            Blue = 100
                        }
                    };

                    simpleFillSymbol.Outline = outline;
                        return (ISymbol)simpleFillSymbol;
                }
            },
            { VisibilityCalculationResultsEnum.VisibilityAreaRaster, (layer) => {


                IRasterStretchColorRampRenderer stretchRen = default(IRasterStretchColorRampRenderer);
                stretchRen = new RasterStretchColorRampRenderer();
                IRasterRenderer pRasRen = default(IRasterRenderer);
                pRasRen = (IRasterRenderer)stretchRen;

                bool bOK;
                IAlgorithmicColorRamp ramp = new AlgorithmicColorRamp();
                ramp.FromColor = new RgbColor()
                {
                    Red = 255,
                    Green = 255,
                    Blue = 115
                };
                ramp.ToColor =  new RgbColor()
                {
                    Red = 115,
                    Green = 38,
                    Blue = 0

                };
                ramp.Algorithm = esriColorRampAlgorithm.esriCIELabAlgorithm;
                            ramp.CreateRamp(out bOK);

                        return null;
                }
            }
        };

        public static void AddResultsToMapAsGroupLayer(
            VisibilityCalcResults results, 
            IActiveView activeView, 
            string relativeLayerName,
            bool isLayerAbove, 
            short transparency, 
            IColor color)
        {
            logger.InfoEx("> AddResultsToMapAsGroupLayer START");

            try
            {
                var visibilityLayers = new List<ILayer>();
                ILayer lr = null;

                foreach (var li in LayersSequence)
                {
                    if (results.ResultsInfo.Any(r => r.RessutType == li.Key))
                    {
                        foreach (var ri in results.ResultsInfo.Where(r => r.RessutType == li.Key))
                        {
                            var dataset = GdbAccess.Instance.GetDatasetFromCalcWorkspace(ri);
                            if (dataset == null)
                            {
                                continue;
                            }

                            if (dataset is IFeatureClass feature)
                            {
                                lr = EsriTools.GetFeatureLayer(feature);
                            }
                            else if (dataset is IRasterDataset raster)
                            {
                                lr = EsriTools.GetRasterLayer(raster);
                            }
                            lr.Visible = li.Value;

                            if (mapResultAction.ContainsKey(ri.RessutType))
                            {
                                mapResultAction[ri.RessutType](lr, color, transparency);
                            }
                            visibilityLayers.Add(lr);
                        }
                    }
                }

                //foreach (var ri in results.ResultsInfo)
                //{
                //    var dataset = GdbAccess.Instance.GetDatasetFromCalcWorkspace(ri);
                //    if (dataset == null)
                //    {
                //        continue;
                //    }

                //    if (dataset is IFeatureClass feature)
                //    {
                //        lr = EsriTools.GetFeatureLayer(feature);
                //    }
                //    if (dataset is IRasterDataset raster)
                //    {
                //        lr = EsriTools.GetRasterLayer(raster);
                //    }

                //    if (mapResultAction.ContainsKey(ri.RessutType))
                //    {
                //        mapResultAction[ri.RessutType](lr, color, transparency);
                //    }

                //    visibilityLayers.Add(lr);

                //}

                MapLayersManager layersManager = new MapLayersManager(activeView);
                //var relativeLayer = layersManager.FirstLevelLayers.FirstOrDefault(l => l.Name.Equals(relativeLayerName, StringComparison.InvariantCultureIgnoreCase));

                ////var relativeLayer = GetLayer(relativeLayerName, activeView.FocusMap);
                //var calcRasters = EsriTools.GetVisibiltyImgLayers(calcRasterName, activeView.FocusMap);

                IGroupLayer groupLayer = new GroupLayerClass { Name = results.Name };

                var layersToremove = new List<IRasterLayer>();
                foreach (var layer in visibilityLayers)
                {
                    if (layer is IRasterLayer raster)
                    {
                        var layerEffects = (ILayerEffects)layer;
                        layerEffects.Transparency = transparency;

                        var existenLayer = 
                            layersManager.RasterLayers.FirstOrDefault(l => l.FilePath.Equals(raster.FilePath, StringComparison.InvariantCultureIgnoreCase));
                        if (existenLayer != null && !layersToremove.Any(l => l.Equals(existenLayer)))
                        {
                            layersToremove.Add(existenLayer);
                        }
                    }
                    groupLayer.Add(layer);
                }
                relativeLayerName = 
                    string.IsNullOrWhiteSpace(relativeLayerName) ? 
                    (layersManager.LastLayer == null ? string.Empty : layersManager.LastLayer.Name) : 
                    relativeLayerName;

                if (!layersManager.InserLayer(groupLayer, relativeLayerName, isLayerAbove))
                {
                    logger.InfoEx("> AddResultsToMapAsGroupLayer END. Cannot add groupped layer {0}", results.Name);
                }
                else
                {
                    logger.InfoEx("> AddResultsToMapAsGroupLayer END. The groupped layer {0} was added", results.Name);
                }

                //var mapLayers = activeView.FocusMap as IMapLayers2;
                //int relativeLayerPosition = GetLayerIndex(relativeLayer, activeView);
                //int groupLayerPosition = (isLayerAbove) ? relativeLayerPosition - 1 : relativeLayerPosition + 1;
                //layersToremove.ForEach(l => mapLayers.DeleteLayer(l));
                //mapLayers.InsertLayer(groupLayer, false, groupLayerPosition);

            }
            catch (Exception ex)
            {
                logger.InfoEx("> AddResultsToMapAsGroupLayer EXCEPTION. Message:{0}", ex.Message);
            }

        }    

    }
}
