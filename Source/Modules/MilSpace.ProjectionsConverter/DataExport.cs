using MilSpace.ProjectionsConverter.Interfaces;
using MilSpace.ProjectionsConverter.Models;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MilSpace.ProjectionsConverter
{
    public class DataExport : IDataExport
    {
        public async Task ExportProjectionsToXmlAsync(PointModel point, string path)
        {
            var xmlSerializer = new XmlSerializer(typeof(PointModel));
            using (var streamWriter = File.Open(path, FileMode.Create))
            {
                using (XmlWriter writer = XmlWriter.Create(streamWriter))
                {
                    xmlSerializer.Serialize(writer, point);
                    await writer.FlushAsync();
                }
            }
        }
    }
}
