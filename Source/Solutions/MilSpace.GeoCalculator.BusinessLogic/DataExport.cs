using MilSpace.GeoCalculator.BusinessLogic.Interfaces;
using MilSpace.GeoCalculator.BusinessLogic.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MilSpace.GeoCalculator.BusinessLogic
{
    public class DataExport : IDataExport
    {
        public async Task ExportProjectionsToXmlAsync(List<PointModel> pointModels, string path)
        {
            var pointModelsList = new PointModelsList() { PointList = pointModels };
            var xmlSerializer = new XmlSerializer(typeof(PointModelsList));
            using (var streamWriter = File.Open(path, FileMode.Create))
            {
                using (XmlWriter writer = XmlWriter.Create(streamWriter, new XmlWriterSettings() { Async = true }))
                {
                    xmlSerializer.Serialize(writer, pointModelsList);
                    await writer.FlushAsync();                    
                }
            }
        }

        public async Task ExportProjectionsToXmlAsync(PointModel pointModel, string path)
        {
            var xmlSerializer = new XmlSerializer(typeof(PointModel));
            using (var streamWriter = File.Open(path, FileMode.Create))
            {
                using (XmlWriter writer = XmlWriter.Create(streamWriter, new XmlWriterSettings() { Async = true }))
                {
                    xmlSerializer.Serialize(writer, pointModel);
                    await writer.FlushAsync();
                }
            }
        }

        public string GetStringRepresentationOfProjections(List<PointModel> pointModels)
        {
            if (pointModels == null || !pointModels.Any()) return string.Empty;

            if (pointModels.Count == 1) return pointModels.First().ToString();

            var stringBuilder = new StringBuilder();

            foreach (var point in pointModels)
            {
                stringBuilder.AppendLine(point.ToString(true));
            }

            return stringBuilder.ToString();
        }
    }
}
