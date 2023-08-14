namespace YahooFinanceAPI.Dto
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
