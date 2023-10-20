using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>This element contains all the authentication results that were evaluated by the receiving system for the given set of messages.</summary>
    public struct RecordType
    {
        [XmlElement("row")]
        public RowType Row { get; set; }
        [XmlElement("identifiers")]
        public IdentifierType Identifiers { get; set; }
        [XmlElement("auth_results")]
        public AuthResultType AuthResults { get; set; }
    }
}
