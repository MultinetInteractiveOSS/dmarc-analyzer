using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>The DMARC-aligned authentication result.</summary>
    public enum DMARCResultType
    {
        [XmlEnum("")]
        Empty,
        [XmlEnum("pass")]
        Pass,
        [XmlEnum("fail")]
        Fail
    }
}
