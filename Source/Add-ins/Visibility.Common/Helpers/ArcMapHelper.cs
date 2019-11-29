using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Visibility
{
    public static class ArcMapHelper
    {

        private static Dictionary<VisibilityCalculationResultsEnum, Func<ISymbol>> mapResultSympols = new Dictionary<VisibilityCalculationResultsEnum, Func<ISymbol>>
        {
            { VisibilityCalculationResultsEnum.VisibilityAreasPotential, () => {

                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                    simpleFillSymbol.Color = new RgbColor()
                    {
                        Transparency = 255
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
            { VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle, () => {

                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                    simpleFillSymbol.Color = new RgbColor()
                    {
                        Transparency = 255
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
            }
        };

        internal static ISymbol GetLegendsForResult(VisibilityCalculationResultsEnum result)
        {
            if ( mapResultSympols.Any(r => r.Key == result))
            {
                return mapResultSympols.First(r => r.Key == result).Value();
            }
            return null;
        }
    }
}
