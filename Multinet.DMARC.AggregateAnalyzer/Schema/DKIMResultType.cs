using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>DKIM verification result, according to RFC 7001 Section 2.6.1.</summary>
    public enum DKIMResultType
    {
        [XmlEnum("")]
        Empty,
        [XmlEnum("none")]
        None,
        [XmlEnum("pass")]
        Pass,
        [XmlEnum("fail")]
        Fail,
        [XmlEnum("policy")]
        Policy,
        [XmlEnum("neutral")]
        Neutral,
        [XmlEnum("temperror")]
        TempError,
        [XmlEnum("permerror")]
        PermError
    }
}
