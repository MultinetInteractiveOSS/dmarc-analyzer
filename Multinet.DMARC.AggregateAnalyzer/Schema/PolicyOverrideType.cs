using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>Reasons that may affect DMARC disposition or execution thereof.</summary>
    public enum PolicyOverrideType
    {
        [XmlEnum("forwarded")]
        Forwarded,
        [XmlEnum("sampled_out")]
        SampledOut,
        [XmlEnum("trusted_forwarder")]
        TrustedForwarder,
        [XmlEnum("mailing_list")]
        MailingList,
        [XmlEnum("local_policy")]
        LocalPolicy,
        [XmlEnum("other")]
        Other
    }
}
