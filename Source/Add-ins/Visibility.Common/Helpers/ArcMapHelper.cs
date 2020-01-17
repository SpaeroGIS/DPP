using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using stdole;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Visibility
{
    public static class ArcMapHelper
    {


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
                { VisibilityCalculationResultsEnum.ObservationPoints, (layer, fillColor, transparency) => {

                 // This example creates a multiLayerMarkerSymbol 
                // that looks like a red circle with a black dropshadow. 


                    // define the necessary variables 
                    IMultiLayerMarkerSymbol multiLayermrkSym = new MultiLayerMarkerSymbol();
                    ICharacterMarkerSymbol charMrkSym1 = new CharacterMarkerSymbol();
                    ICharacterMarkerSymbol charMrkSym2 = new CharacterMarkerSymbol();


                    IRgbColor foreColor = new RgbColor();
                    IRgbColor backColor = new RgbColor();

                    //    stdole.IFontDisp tFont = ESRI.ArcGIS.ADF.Connection.Local.Converter.ToStdFont(new Font("ESRI Default Marker", 18));

                    IFontDisp tFont = (IFontDisp)(new StdFont());


                    // Create a reference to the font that contains the circle glyphs 
                    tFont.Name = "ESRI Default Marker";
                    tFont.Size =  18;


    // Create the red and black colors 
                    foreColor.Red = 0;
                    foreColor.Green = 0;
                    foreColor.Blue = 0;
                    backColor.Red = 0;
                    backColor.Green = 255;
                    backColor.Blue = 0;


                    // Create the Markers 
                    charMrkSym1.Angle = 0;
                    charMrkSym1.CharacterIndex = 49;
                    charMrkSym1.Color = foreColor;
                    charMrkSym1.Font = tFont;
                    charMrkSym1.Size = 18;
                    charMrkSym1.XOffset = 0;
                    charMrkSym1.YOffset = 0;

                    charMrkSym2.Angle = 0;
                    charMrkSym2.CharacterIndex = 36;
                    charMrkSym2.Color = backColor;
                    charMrkSym2.Font = tFont;
                    charMrkSym2.Size = 18;
                    charMrkSym2.XOffset = 0;
                    charMrkSym2.YOffset = 0;


                    // Add the symbols in the order of bottommost to topmost 
                    multiLayermrkSym.AddLayer(charMrkSym2);
                    multiLayermrkSym.AddLayer(charMrkSym1);
                    multiLayermrkSym.Angle = 0;
                    multiLayermrkSym.Size = 18;
                    multiLayermrkSym.XOffset = 0;
                    multiLayermrkSym.YOffset = 0;
                    EsriTools.SetFeatureLayerStyle(layer as IFeatureLayer,  multiLayermrkSym as ISymbol);
                }
            },
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
                        logger.InfoEx($"Setting unique values for renderring \"{rasterLayer.Name}\" ");
                        try
                        {
                            var render = EsriTools.GetCalclResultRender(rasterLayer.Raster, "Value");
                            if (render == null)
                            {
                                logger.ErrorEx($"The raster \"{rasterLayer.Name}\" doesn't have a table");
                            }

                            rasterLayer.Renderer = render;
                            logger.InfoEx($"Unique values for renderring \"{rasterLayer.Name}\" was set");
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
                         logger.InfoEx($"Setting unique values for renderring \"{rasterLayer.Name}\" ");
                        try
                        {
                            var render = EsriTools.GetCalclResultRender(rasterLayer.Raster, "Value");
                            if (render == null)
                            {
                                logger.ErrorEx($"The raster \"{rasterLayer.Name}\" doesn't have a table");
                            }

                            rasterLayer.Renderer = render;
                            logger.InfoEx($"Unique values for renderring \"{rasterLayer.Name}\" was set");
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
                        logger.InfoEx($"Setting unique values for renderring \"{rasterLayer.Name}\" ");
                        try
                        {
                            var render = EsriTools.GetCalclResultRender(rasterLayer.Raster, "Value");
                            if (render == null)
                            {
                                logger.ErrorEx($"The raster \"{rasterLayer.Name}\" doesn't have a table");
                            }

                            rasterLayer.Renderer = render;
                            logger.InfoEx($"Unique values for renderring \"{rasterLayer.Name}\" was set");
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
            { VisibilityCalculationResultsEnum.VisibilityAreaTrimmedByPolySingle, (layer, fillColor, transparency) => {

                  if (layer is IRasterLayer rasterLayer )
                    {
                        logger.InfoEx($"Setting unique values for renderring \"{rasterLayer.Name}\" ");
                        try
                        {
                            var render = EsriTools.GetCalclResultRender(rasterLayer.Raster, "Value");
                            if (render == null)
                            {
                                logger.ErrorEx($"The raster \"{rasterLayer.Name}\" doesn't have a table");
                            }

                            rasterLayer.Renderer = render;
                            logger.InfoEx($"Unique values for renderring \"{rasterLayer.Name}\" was set");
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
                        logger.InfoEx($"Setting unique values for renderring \"{polygonLayer.Name}\" ");
                        try
                        {
                            var render = EsriTools.GetCalclResultRender(polygonLayer, "gridcode");
                            if (render == null)
                            {
                                logger.ErrorEx($"The raster \"{polygonLayer.Name}\" doesn't have a table");
                            }

                            IGeoFeatureLayer geoFeatureLayer = (IGeoFeatureLayer)polygonLayer;
                            geoFeatureLayer.Renderer = render;
                            logger.InfoEx($"Unique values for renderring \"{polygonLayer.Name}\" was set");
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
            { VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle, (layer, fillColor, transparency) => {

                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                    simpleFillSymbol.Color = new RgbColor()
                    {
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

                MapLayersManager layersManager = new MapLayersManager(activeView);

                IGroupLayer groupLayer = new GroupLayerClass { Name = results.Name };

                var layersToremove = new List<IRasterLayer>();
                foreach (var layer in visibilityLayers)
                {
                    if (layer is IRasterLayer raster)
                    {
                        var existenLayer =
                            layersManager.RasterLayers.FirstOrDefault(l => l.FilePath.Equals(raster.FilePath, StringComparison.InvariantCultureIgnoreCase));
                        if (existenLayer != null && !layersToremove.Any(l => l.Equals(existenLayer)))
                        {
                            layersToremove.Add(existenLayer);
                        }
                    }
                    var layerEffects = (ILayerEffects)layer;
                    layerEffects.Transparency = transparency;
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
