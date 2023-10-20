using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>Alignment mode (relaxed or strict) for DKIM and SPF.</summary>
    public enum AlignmentType
    {
        [XmlEnum("r")]
        Relaxed,
        [XmlEnum("s")]
        Strict
    }
}
