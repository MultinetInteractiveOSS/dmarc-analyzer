using System.Collections.Generic;
using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>Taking into account everything else in the record, the results of applying DMARC.</summary>
    public class PolicyEvaluatedType
    {
        [XmlElement("disposition")]
        public DispositionType Disposition { get; set; }
        [XmlElement("dkim")]
        public DMARCResultType DKIM { get; set; }
        [XmlElement("spf")]
        public DMARCResultType SPF { get; set; }
        [XmlElement("reason")]
        public List<PolicyOverrideReason> PolicyOverrideReason { get; set; }
    }
}
