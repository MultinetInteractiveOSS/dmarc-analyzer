using Multinet.DMARC.AggregateAnalyzer.Schema;
using System.IO;
using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer
{
    public static class Parser
    {
        public static DMARCReport ParseXML(string xml)
        {
            var serializer = new XmlSerializer(typeof(DMARCReport));
            using (var reader = new StringReader(xml))
            {
                return serializer.Deserialize(reader) as DMARCReport;
            }
        }

        public static DMARCReport ParseFile(string path)
        {
            var serializer = new XmlSerializer(typeof(DMARCReport));
            using (var reader = new StreamReader(path))
            {
                return serializer.Deserialize(reader) as DMARCReport;
            }
        }
    }
}
