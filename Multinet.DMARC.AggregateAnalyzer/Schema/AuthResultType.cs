using System.Collections.Generic;
using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>This element contains DKIM and SPF results, uninterpreted with respect to DMARC.</summary>
    public class AuthResultType
    {
        [XmlElement("dkim")]
        public List<DKIMAuthResultType> DKIM { get; set; }
        [XmlElement("spf")]
        public List<SPFAuthResultType> SPF { get; set; }
    }
}
