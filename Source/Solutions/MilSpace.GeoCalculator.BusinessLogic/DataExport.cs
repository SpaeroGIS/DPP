using MilSpace.GeoCalculator.BusinessLogic.Interfaces;
using MilSpace.GeoCalculator.BusinessLogic.Models;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MilSpace.GeoCalculator.BusinessLogic
{
    public class DataExport : IDataExport
    {
        public async Task ExportProjectionsToXmlAsync(PointModel point, string path)
        {
            var xmlSerializer = new XmlSerializer(typeof(PointModel));
            using (var streamWriter = File.Open(path, FileMode.Create))
            {
                using (XmlWriter writer = XmlWriter.Create(streamWriter, new XmlWriterSettings() { Async = true }))
                {
                    xmlSerializer.Serialize(writer, point);
                    await writer.FlushAsync();
                }
            }
        }
    }
}
