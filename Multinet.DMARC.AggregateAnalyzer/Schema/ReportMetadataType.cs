using System.Collections.Generic;
using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>Report generator metadata.</summary>
    public class ReportMetadataType
    {
        [XmlElement("org_name")]
        public string OrganizationName { get; set; }
        [XmlElement("email")]
        public string Email { get; set; }
        [XmlElement("extra_contact_info")]
        public string ExtraContactInfo { get; set; }
        [XmlElement("report_id")]
        public string ReportId { get; set; }
        [XmlElement("date_range")]
        public DateRangeType DateRange { get; set; }
        [XmlElement("error")]
        public List<string> Error { get; set; } = new List<string>();
    }
}
