namespace YahooFinanceAPI.Models
{
    // Models/EMAData.cs
    public class EMAData
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public int Period { get; set; }

        public Company Company { get; set; }
    }
}
