﻿using System.Text.Json.Serialization;

namespace YahooFinanceAPI.Models
{
    public class EODData
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime Date { get; set; }
        public decimal? Open { get; set; }
        public decimal? High { get; set; }
        public decimal? Low { get; set; }
        public decimal? Close { get; set; }
        public long? Volume { get; set; }

        [JsonIgnore]
        public Company Company { get; set; }
    }
}
