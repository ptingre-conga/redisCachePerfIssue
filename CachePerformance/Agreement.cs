using System;

namespace CachePerformance
{
    public class Agreement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AgreementNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string StatusCategory { get; set; }
        public string TotalAgreementValue { get; set; }
    }
}
