using System.Collections.Generic;
using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    [XmlRoot("feedback", IsNullable = false)]
    public class DMARCReport
    {
        [XmlElement("version")]
        public decimal Version { get; set; }

        [XmlElement("report_metadata")]
        public ReportMetadataType ReportMetadata { get; set; }

        [XmlElement("policy_published")]
        public PolicyPublishedType PolicyPublished { get; set; }

        [XmlElement("record")]
        public List<RecordType> Records { get; set; }
    }
}
