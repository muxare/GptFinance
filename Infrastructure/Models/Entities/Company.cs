namespace GptFinance.Infrastructure.Models.Entities
{
    // Models/Company.cs
    public class Company
    {
        public Guid Id { get; set; }
        public string? Symbol { get; set; }
        public string? Name { get; set; }
        public DateTime LastUpdated { get; set; }

        //public StockExchange StockExchange { get; set; }
        public ICollection<EodData> EodData { get; set; } = new List<EodData>();
    }
}
