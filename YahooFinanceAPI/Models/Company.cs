namespace YahooFinanceAPI.Models
{
    // Models/Company.cs
    public class Company
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public DateTime LastUpdated { get; set; }

        public ICollection<EODData> EODData { get; set; } = new List<EODData>();
    }
}
