using MilSpace.GeoCalculator.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator.BusinessLogic.Extensions
{
    public static class CollectionsExtension
    {
        public static List<PointModel> ToSortedPointModelsList(this IDictionary<string, PointModel> pointModelDictionary)
        {
            return pointModelDictionary.Values.OrderBy(value => value.PointNumber).ToList();
        }
    }
}
