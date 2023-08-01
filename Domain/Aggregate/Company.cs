using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptFinance.Domain.Aggregate
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public FinancialData FinancialData { get; set; }
        public DateTime LastUpdate { get; set; }

        public Company()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            Symbol = string.Empty;
            LastUpdate = DateTime.Now;
            FinancialData = new FinancialData();
        }
    }
}
