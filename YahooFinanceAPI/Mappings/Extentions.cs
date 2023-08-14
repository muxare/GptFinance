using GptFinance.Domain.Aggregate;
using YahooFinanceAPI.Dto;

namespace YahooFinanceAPI.Mappings
{
    public static class Extentions
    {
        public static CompanyDto Map(this CompanyAggregate company)
        {
            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Symbol = company.Symbol,
                //LastUpdated = company.LastUpdate,
                //EodData = (ICollection<Models.Entities.EodData>)company.FinancialData.EodData
            };
        }
    }
}
