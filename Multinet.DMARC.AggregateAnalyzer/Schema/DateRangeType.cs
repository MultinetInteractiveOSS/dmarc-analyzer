using System;
using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer.Schema
{
    /// <summary>The time range in UTC covered by messages in this report, specified in seconds since epoch.</summary>
    public struct DateRangeType
    {
        [XmlElement("begin")]
        public long Begin { get; set; }
        public DateTimeOffset BeginDate
        {
            get { return DateTimeOffset.FromUnixTimeSeconds(Begin); }
        }
        [XmlElement("end")]
        public long End { get; set; }
        public DateTimeOffset EndDate
        {
            get { return DateTimeOffset.FromUnixTimeSeconds(End); }
        }
    }
}
