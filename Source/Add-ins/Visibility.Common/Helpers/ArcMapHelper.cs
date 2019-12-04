using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
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

        static Logger logger = Logger.GetLoggerEx("Visibility Addin Helper");


        private static Dictionary<VisibilityCalculationResultsEnum, Action<ILayer, IColor, short>> mapResultAction = new Dictionary<VisibilityCalculationResultsEnum, Action<ILayer, IColor, short>>
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
                         IRaster pRaster = default(IRaster);
                        pRaster = rasterLayer.Raster;

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
                        ramp.Size = 255;
                        ramp.Algorithm = esriColorRampAlgorithm.esriCIELabAlgorithm;
                        ramp.CreateRamp(out bOK);

                        stretchRen.BandIndex = 0;
                        stretchRen.ColorRamp = ramp;
                        pRasRen.Update();
                        rasterLayer.Renderer = (IRasterRenderer)stretchRen;

                    }
                }
            },
            { VisibilityCalculationResultsEnum.VisibilityObservStationClip, (layer, fillColor, transparency) => {

                    if (layer is IRasterLayer rasterLayer )
                    {
                         IRaster pRaster = default(IRaster);
                        pRaster = rasterLayer.Raster;

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
                    try
                    {
                        ramp.CreateRamp(out bOK);

                        stretchRen.BandIndex = 0;
                        stretchRen.ColorRamp = ramp;
                        pRasRen.Update();
                        rasterLayer.Renderer = (IRasterRenderer)stretchRen;
                        }
                    catch
                    {

                    }

                    }
                }
            },
            { VisibilityCalculationResultsEnum.VisibilityAreaRaster, (layer, fillColor, transparency) => {

                    if (layer is IRasterLayer rasterLayer )
                    {
                         IRaster pRaster = default(IRaster);
                        pRaster = rasterLayer.Raster;

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

                        stretchRen.BandIndex = 0;
                        stretchRen.ColorRamp = ramp;
                        pRasRen.Update();
                        rasterLayer.Renderer = (IRasterRenderer)stretchRen;

                    }
                }
            },
            { VisibilityCalculationResultsEnum.VisibilityAreaPolygons, (layer, fillColor, transparency) => {

                    //if (layer is IFeatureLayer polygonLayer )
                    //{
                    //        IColorRampSymbol  colorRampSymbol = new ColorRampSymbol();
                    //        IAlgorithmicColorRamp algColorRamp = new AlgorithmicColorRampClass();


                    //        //Create the start and end colors
                    //        IRgbColor startColor = new RgbColor()
                    //        {
                    //            Red = 255,
                    //            Green = 255,
                    //            Blue = 115
                    //        };
                    //        IRgbColor endColor = new RgbColor()
                    //        {
                    //            Red = 115,
                    //            Green = 38,
                    //            Blue = 0

                    //        };
                           
                    //        //Set the Start and End Colors
                    //        algColorRamp.ToColor = startColor;
                    //        algColorRamp.FromColor = endColor;

                    //        //Set the ramping Alglorithm 
                    //        algColorRamp.Algorithm = esriColorRampAlgorithm.esriCIELabAlgorithm;

                    //        //Set the size of the ramp (the number of colors to be derived)
                    //        algColorRamp.Size = 255;


                    //        //Create the ramp
                    //        bool ok = true;
                    //        algColorRamp.CreateRamp(out ok);


                    //    if (ok)
                    //        colorRampSymbol.ColorRamp =algColorRamp;

                    ////ICartographicLineSymbol outline = new CartographicLineSymbol
                    ////{
                    ////    Width = 0.4,
                    ////    Color = new RgbColor()
                    ////    {
                    ////        Red = 100,
                    ////        Green = 100,
                    ////        Blue = 100
                    ////    }
                    ////};

                    ////colorRampSymbol.Outline = outline;
                    //EsriTools.SetFeatureLayerStyle(layer as IFeatureLayer,  colorRampSymbol as ISymbol);
                    //}
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

        public static void AddResultsToMapAsGroupLayer(VisibilityCalcResults results, IActiveView activeView, string relativeLayerName,
                                                bool isLayerAbove, short transparency, IColor color)

        {
            var visibilityLayers = new List<ILayer>();
            ILayer lr = null;

            foreach (var ri in results.ResultsInfo)
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
                if (dataset is IRasterDataset raster)
                {
                    lr = EsriTools.GetRasterLayer(raster);
                }

                if (mapResultAction.ContainsKey(ri.RessutType))
                {
                    mapResultAction[ri.RessutType](lr, color, transparency);
                }

                visibilityLayers.Add(lr);

            }

            MapLayersManager layersManager = new MapLayersManager(activeView);
            //var relativeLayer = layersManager.FirstLevelLayers.FirstOrDefault(l => l.Name.Equals(relativeLayerName, StringComparison.InvariantCultureIgnoreCase));

            ////var relativeLayer = GetLayer(relativeLayerName, activeView.FocusMap);
            //var calcRasters = EsriTools.GetVisibiltyImgLayers(calcRasterName, activeView.FocusMap);

            IGroupLayer groupLayer = new GroupLayerClass
            { Name = results.Name };

            var layersToremove = new List<IRasterLayer>();
            foreach (var layer in visibilityLayers)
            {
                if (layer is IRasterLayer raster)
                {
                    var layerEffects = (ILayerEffects)layer;
                    layerEffects.Transparency = transparency;

                    var existenLayer = layersManager.RasterLayers.FirstOrDefault(l => l.FilePath.Equals(raster.FilePath, StringComparison.InvariantCultureIgnoreCase));
                    if (existenLayer != null && !layersToremove.Any(l => l.Equals(existenLayer)))
                    {
                        layersToremove.Add(existenLayer);
                    }

                }

                groupLayer.Add(layer);
            }

            relativeLayerName = string.IsNullOrWhiteSpace(relativeLayerName) ? (layersManager.LastLayer == null ? string.Empty : layersManager.LastLayer.Name) : relativeLayerName;

            if (!layersManager.InserLayer(groupLayer, relativeLayerName, isLayerAbove))
            {
                logger.ErrorEx($"Cannot add groupped layer {results.Name} near the layer {relativeLayerName}");
            }
            else
            {
                logger.InfoEx($"The groupped layer {results.Name} was added near the layer {relativeLayerName}");
            }

            //var mapLayers = activeView.FocusMap as IMapLayers2;
            //int relativeLayerPosition = GetLayerIndex(relativeLayer, activeView);
            //int groupLayerPosition = (isLayerAbove) ? relativeLayerPosition - 1 : relativeLayerPosition + 1;



            //layersToremove.ForEach(l => mapLayers.DeleteLayer(l));
            //mapLayers.InsertLayer(groupLayer, false, groupLayerPosition);
        }
    }
}
