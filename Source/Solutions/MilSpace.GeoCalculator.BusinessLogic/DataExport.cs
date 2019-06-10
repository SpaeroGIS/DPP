using MilSpace.GeoCalculator.BusinessLogic.Interfaces;
using MilSpace.GeoCalculator.BusinessLogic.Models;
using System.Collections.Generic;
using System.IO;
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

        public string GetXmlRepresentationOfProjections(List<PointModel> pointModels)
        {
            var pointModelsList = new PointModelsList() { PointList = pointModels };
            var xmlSerializer = new XmlSerializer(typeof(PointModelsList));
            using (var stringWriter = new System.IO.StringWriter())
            {                
                xmlSerializer.Serialize(stringWriter, pointModelsList);
                return stringWriter.ToString();
            }            
        }
    }
}
