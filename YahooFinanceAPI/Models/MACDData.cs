namespace YahooFinanceAPI.Models
{
    // Models/MACDData.cs
    public class MACDData
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public int ShortPeriod { get; set; }
        public int LongPeriod { get; set; }
        public int SignalPeriod { get; set; }

        public Company Company { get; set; }
    }
}
