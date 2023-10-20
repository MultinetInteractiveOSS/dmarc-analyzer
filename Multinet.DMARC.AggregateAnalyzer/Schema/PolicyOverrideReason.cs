using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>How do we allow report generators to include new classes of override reasons if they want to be more specific than "other"?</summary>
    public struct PolicyOverrideReason
    {
        [XmlElement("type")]
        public PolicyOverrideType Type { get; set; }
        [XmlElement("comment")]
        public string Comment { get; set; }
    }
}
