using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    public struct RowType
    {
        [XmlElement("source_ip")]
        public string SourceIp { get; set; }
        [XmlElement("count")]
        public int Count { get; set; }
        [XmlElement("policy_evaluated")]
        public PolicyEvaluatedType PolicyEvaluated { get; set; }
    }
}
