using DomainAggregate = GptFinance.Domain.Aggregate;
using DomainEntity = GptFinance.Domain.Entity;
using GptFinance.Infrastructure.Models.Entities;

using GptFinance.Domain.Entity;

using System.Linq.Expressions;

using GptFinance.Domain.Aggregate;

using GptFinance.Application.Models;
using GptFinance.Application.Models.Dto;

namespace GptFinance.Infrastructure.Mappings
{
    public static class MapperExtentions
    {
        public static EodDomainEntity Map(this EodData source)
        {
            return new EodDomainEntity(source.Date, source.Open ?? null, source.High ?? null, source.Low ?? null, source.Close ?? null, source.Close ?? null, (int?)(source.Volume ?? source.Volume));
        }

        public static ValueTask<EodDomainEntity?> Map(this ValueTask<EodData?> task)
        {
            // Logic to transform the task's result goes here

            // For example, you might do something like:
            return new ValueTask<EodDomainEntity?>(task.Result.Map());
        }

        public static EmaDomainEntity Map(this EmaData source)
        {
            return new EmaDomainEntity(source.Date, source.Period, source.Value);
        }

        public static MacdDomainEntity Map(this MacdData source)
        {
            return new MacdDomainEntity(source.Date, source.ShortPeriod, source.LongPeriod, source.SignalPeriod, source.MacdValue, source.SignalValue, source.HistogramValue);
        }

        public static Models.Entities.Company Map(this DomainAggregate.CompanyAggregate company)
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

        public static DomainAggregate.CompanyAggregate Map(this Models.Entities.Company company)
        {
            return new DomainAggregate.CompanyAggregate()
            {
                Id = company.Id,
                Name = company.Name ?? string.Empty,
                Symbol = company.Symbol ?? string.Empty,
                LastUpdate = company.LastUpdated,
                FinancialData = new DomainAggregate.FinancialDataAggregate
                {
                    EodData = (ICollection<Domain.Entity.EodDomainEntity>)company.EodData
                }
            };
        }

        public static Models.Entities.EmaData Map(this DomainEntity.EmaDomainEntity ema)
        {
            return new Models.Entities.EmaData()
            {
                Date = ema.Date,
                Period = ema.Window,
                Value = ema.Value.Value
            };
        }

        /*public static DomainEntity.EmaDomainEntity Map(this Models.Entities.EmaData ema)
        {
            return new DomainEntity.EmaDomainEntity(Date: ema.Date, Window: ema.Period, Value: ema.Value);
        }*/

        public static Models.Entities.MacdData Map(this DomainEntity.MacdDomainEntity macd)
        {
            return new Models.Entities.MacdData()
            {
                Date = macd.Date,
                ShortPeriod = macd.FastWindow,
                LongPeriod = macd.SlowWindow,
                SignalPeriod = macd.SignalWindow,
                MacdValue = macd.MacdValue.Value,
                SignalValue = macd.SignalValue.Value,
                HistogramValue = macd.HistogramValue.Value
            };
        }

        public static Models.Entities.EodData Map(this DomainEntity.EodDomainEntity eod)
        {
            return new Models.Entities.EodData()
            {
                Date = eod.Date,
                Open = eod.Open,
                High = eod.High,
                Low = eod.Low,
                Close = eod.Close,
                Volume = eod.Volume
            };
        }

        //public static Models.Entities.StockExchange Map(this StockExchangeAggregate o)
        //{
        //    return new StockExchange()
        //    {
        //        Id = o.Id,
        //        Name = o.Name,
        //        Marketplace = o.Marketplace,
        //        Ranking = o.Ranking,
        //        TimeZone = o.TimeZone,
        //        TradingHours = o.TradingHours,
        //        LunchBreak = o.LunchBreak
        //    };
        //}

        //public static StockExchangeAggregate Map(this StockExchange o)
        //{
        //    return new StockExchangeAggregate()
        //    {
        //        Id = o.Id,
        //        Name = o.Name,
        //        Marketplace = o.Marketplace,
        //        Ranking = o.Ranking,
        //        TimeZone = o.TimeZone,
        //        TradingHours = o.TradingHours,
        //        LunchBreak = o.LunchBreak
        //    };
        //}

        /*public static DomainEntity.MacdDomainEntity Map(this Models.Entities.MacdData macd)
        {
            return new DomainEntity.MacdDomainEntity(
                Date: macd.Date,
                FastWindow: macd.ShortPeriod,
                SlowWindow: macd.LongPeriod,
                SignalWindow: macd.SignalPeriod,
                MacdValue: macd.MacdValue,
                SignalValue: macd.SignalValue,
                HistogramValue: macd.HistogramValue);
        }*/

        public static CompanyAggregate Map(this CompanySearchDto source)
        {
            return new CompanyAggregate
            {
                Name = source.LongName,
                Symbol = source.Symbol,
                StockExchange = new StockExchangeAggregate
                {
                }
            };
        }

        public static Expression<Func<MacdData, bool>> ConvertExpression(Expression<Func<MacdDomainEntity, bool>> expression)
        {
            // Parameter for the new expression
            var parameter = Expression.Parameter(typeof(MacdData), "x");

            // Mapping between MacdDomainEntity properties and MacdData properties
            var propertyMap = new Dictionary<string, string>
            {
                { "Date", "Date" },
                { "MacdValue", "MacdValue" },
                { "SignalValue", "SignalValue" },
                { "HistogramValue", "HistogramValue" },
                { "FastWindow", "ShortPeriod" },
                { "SlowWindow", "LongPeriod" },
                { "SignalWindow", "SignalPeriod" }
            };

            // Replace the parameter and properties in the original expression
            var body = new ExpressionReplacer(parameter, propertyMap).Visit(expression.Body);

            // Create the new expression
            return Expression.Lambda<Func<MacdData, bool>>(body, parameter);
        }

        public static Expression<Func<EmaData, bool>> ConvertExpression(Expression<Func<EmaDomainEntity, bool>> expression)
        {
            // Parameter for the new expression
            var parameter = Expression.Parameter(typeof(MacdData), "x");

            // Mapping between MacdDomainEntity properties and MacdData properties
            var propertyMap = new Dictionary<string, string>
            {
                { "Date", "Date" },
                { "Window", "Period" },
                { "Value", "Value" }
            };

            // Replace the parameter and properties in the original expression
            var body = new ExpressionReplacer(parameter, propertyMap).Visit(expression.Body);

            // Create the new expression
            return Expression.Lambda<Func<EmaData, bool>>(body, parameter);
        }

        private class ExpressionReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _parameter;
            private readonly Dictionary<string, string> _propertyMap;

            public ExpressionReplacer(ParameterExpression parameter, Dictionary<string, string> propertyMap)
            {
                _parameter = parameter;
                _propertyMap = propertyMap;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return _parameter; // Replace the parameter
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                // Replace the property if it's in the mapping
                if (_propertyMap.TryGetValue(node.Member.Name, out var mappedProperty))
                {
                    return Expression.Property(Visit(node.Expression), mappedProperty);
                }

                return base.VisitMember(node);
            }
        }
    }
}
