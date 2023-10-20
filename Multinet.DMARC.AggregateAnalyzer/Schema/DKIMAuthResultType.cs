using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    public struct DKIMAuthResultType
    {
        [XmlElement("domain")]
        public string Domain { get; set; }
        [XmlElement("selector")]
        public string Selector { get; set; }
        [XmlElement("result")]
        public DKIMResultType Result { get; set; }
        [XmlElement("human_result")]
        public string HumanResult { get; set; }
    }
}
