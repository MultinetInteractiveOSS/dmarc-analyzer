using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>SPF domain scope.</summary>
    public enum SPFDomainScope
    {
        [XmlEnum("helo")]
        HELO,
        [XmlEnum("mfrom")]
        MailFrom
    }
}
