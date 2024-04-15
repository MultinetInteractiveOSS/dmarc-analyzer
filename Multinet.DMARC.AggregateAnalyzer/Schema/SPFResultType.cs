using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>SPF result.</summary>
    public enum SPFResultType
    {
        [XmlEnum("")]
        Empty,
        [XmlEnum("none")]
        None,
        [XmlEnum("neutral")]
        Neutral,
        [XmlEnum("pass")]
        Pass,
        [XmlEnum("fail")]
        Fail,
        [XmlEnum("softfail")]
        SoftFail,
        [XmlEnum("temperror")]
        TempError,
        [XmlEnum("permerror")]
        PermError,
        [XmlEnum("unknown")]
        Unknown
    }
}
