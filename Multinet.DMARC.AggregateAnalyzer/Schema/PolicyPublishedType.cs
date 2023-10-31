using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>The DMARC policy that applied to the messages in this report.</summary>
    public class PolicyPublishedType
    {
        [XmlElement("domain")]
        public string Domain { get; set; }
        [XmlElement("adkim")]
        public AlignmentType DKIMAlignmentMode { get; set; }
        [XmlElement("aspf")]
        public AlignmentType SPFAlignmentMode { get; set; }
        [XmlElement("p")]
        public DispositionType Policy { get; set; }
        [XmlElement("sp")]
        public DispositionType SubdomainPolicy { get; set; }
        [XmlElement("pct")]
        public int Percent { get; set; }
        [XmlElement("fo")]
        public string FailureReportingOptions { get; set; }
    }
}
