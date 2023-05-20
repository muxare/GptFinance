namespace GptFinance.Domain.Entities
{
    // Models/MACDData.cs
    public class MacdData
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime Date { get; set; }
        public decimal MacdValue { get; set; }
        public decimal SignalValue { get; set; }
        public decimal HistogramValue { get; set; }
        public int ShortPeriod { get; set; }
        public int LongPeriod { get; set; }
        public int SignalPeriod { get; set; }
    }
}
