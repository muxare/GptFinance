using GptFinance.Domain.Entity;

namespace GptFinance.Domain.Aggregate
{
    /// <summary>
    /// TODO: In the update methods only add new data, don't update existing data
    /// </summary>
    public class CompanyAggregate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public FinancialDataAggregate FinancialData { get; set; }
        public StockExchangeAggregate StockExchange { get; set; }
        public DateTime LastUpdate { get; set; }

        public CompanyAggregate()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            Symbol = string.Empty;
            LastUpdate = DateTime.Now;
            FinancialData = new FinancialDataAggregate();
        }

        public bool InUptrend()
        {
            // Calculate emafandata emadata

            var emaFanLatest = FinancialData.EmaData.OrderByDescending(o => o.Date).Take(4);
            var uptrend = emaFanLatest.OrderBy(o => o.Value).ToList();

            return FinancialData.EodData.Count > 0 && FinancialData.EodData.Last().Close > FinancialData.EmaData.Last().Value;
        }

        public void UpdateEodData(ICollection<EodDomainEntity> entities)
        {
            FinancialData.EodData = entities;
        }

        public void UpdateEmaData(ICollection<EmaDomainEntity> entities)
        {
            FinancialData.EmaData = entities;
        }

        public void UpdateMacdData(ICollection<MacdDomainEntity> entities)
        {
            FinancialData.MacdData = entities;
        }

        public bool MarketIsClosed()
        {
            var now = DateTime.UtcNow;
            var weekend = now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday;
            var open = StockExchange.TradingHours.Close < now.TimeOfDay;
            return weekend || open;
        }
    }
}
