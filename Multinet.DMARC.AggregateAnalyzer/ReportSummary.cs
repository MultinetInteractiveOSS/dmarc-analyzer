using Multinet.DMARC.AggregateAnalyzer.Schema;
using System;
using System.Collections.Generic;

namespace Multinet.DMARC.AggregateAnalyzer
{
    public class ReportSummary
    {
        /// <summary>Total volume of emails</summary>
        public long TotalVolume { get; set; }

        public long DMARCVolume { get; set; }
        public double DMARCPercent
        {
            get
            {
                return Math.Round(DMARCVolume / (double)TotalVolume, 4);
            }
        }

        public long DKIMVolume { get; set; }
        public double DKIMPercent
        {
            get
            {
                return Math.Round(DKIMVolume / (double)TotalVolume, 4);
            }
        }

        public long SPFVolume { get; set; }
        public double SPFPercent
        {
            get
            {
                return Math.Round(SPFVolume / (double)TotalVolume, 4);
            }
        }

        public long ForwarderVolume { get; set; }
        public double ForwarderPercent
        {
            get
            {
                return Math.Round(ForwarderVolume / (double)TotalVolume, 4);
            }
        }

        public long UnknownVolume { get; set; }
        public double UnknownPercent
        {
            get
            {
                return Math.Round(UnknownVolume / (double)TotalVolume, 4);
            }
        }

        /// <summary>Sources who are DMARC capable</summary>
        public Dictionary<string, List<RecordType>> DMARC { get; set; }
        /// <summary>Sources who are DMARC capable</summary>
        public Dictionary<string, List<RecordType>> Forwarder { get; set; }
        /// <summary>Sources that are unknown to us</summary>
        public Dictionary<string, List<RecordType>> Unknown { get; set; }
    }
}
