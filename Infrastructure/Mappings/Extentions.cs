using DomainAggregate = GptFinance.Domain.Aggregate;
using DomainEntity = GptFinance.Domain.Entity;
using GptFinance.Infrastructure.Models.Entities;

namespace GptFinance.Infrastructure.Mappings
{
    public static class MapperExtentions
    {
        public static Models.Entities.Company Map(this DomainAggregate.Company company)
        {
            return new Models.Entities.Company()
            {
                Id = company.Id,
                Name = company.Name,
                Symbol = company.Symbol,
                LastUpdated = company.LastUpdate,
                EodData = (ICollection<Models.Entities.EodData>)company.FinancialData.EodData
            };
        }

        public static DomainAggregate.Company Map(this Models.Entities.Company company)
        {
            return new DomainAggregate.Company()
            {
                Id = company.Id,
                Name = company.Name ?? string.Empty,
                Symbol = company.Symbol ?? string.Empty,
                LastUpdate = company.LastUpdated,
                FinancialData = new DomainAggregate.FinancialData
                {
                    EodData = (ICollection<Domain.Entity.Eod>)company.EodData
                }
            };
        }

        public static Models.Entities.EmaData Map(this DomainEntity.Ema ema)
        {
            return new Models.Entities.EmaData()
            {
            };
        }

        public static DomainEntity.Ema Map(this Models.Entities.EmaData ema)
        {
            return new DomainEntity.Ema(Date: ema.Date, Window: ema.Period, Value: ema.Value);
        }

        public static Models.Entities.MacdData Map(this DomainEntity.Macd macd)
        {
            return new Models.Entities.MacdData()
            {
                Date = macd.Date,
            };
        }

        public static DomainEntity.Macd Map(this Models.Entities.MacdData macd)
        {
            return new DomainEntity.Macd(
                Date: macd.Date,
                FastWindow: macd.ShortPeriod,
                SlowWindow: macd.LongPeriod,
                SignalWindow: macd.SignalPeriod,
                MacdValue: macd.MacdValue,
                SignalValue: macd.SignalValue,
                HistogramValue: macd.HistogramValue);
        }
    }
}
