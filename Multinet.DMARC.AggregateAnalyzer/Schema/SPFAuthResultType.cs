using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    public class SPFAuthResultType
    {
        [XmlElement("domain")]
        public string Domain { get; set; }
        [XmlElement("scope")]
        public SPFDomainScope Scope { get; set; }
        [XmlElement("result")]
        public SPFResultType Result { get; set; }
    }
}
