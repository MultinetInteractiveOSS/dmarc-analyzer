using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    public struct IdentifierType
    {
        [XmlElement("envelope_to")]
        public string EnvelopeTo { get; set; }
        [XmlElement("envelope_from")]
        public string EnvelopeFrom { get; set; }
        [XmlElement("header_from")]
        public string HeaderFrom { get; set; }
    }
}
