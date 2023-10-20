using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>The policy actions specified by p and sp in the DMARC record.</summary>
    public enum DispositionType
    {
        [XmlEnum("")]
        Null,
        [XmlEnum("none")]
        None,
        [XmlEnum("quarantine")]
        Quarantine,
        [XmlEnum("reject")]
        Reject
    }
}
