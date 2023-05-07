using CsvHelper.Configuration;
using GptFinance.Infrastructure.Converters;
using GptFinance.Infrastructure.Models;

namespace GptFinance.Infrastructure.Mappings
{
    public sealed class CsvRecordMap : ClassMap<CsvRecord>
    {
        public CsvRecordMap()
        {
            Map(m => m.Date).Name("Date");
            Map(m => m.Open).Name("Open").TypeConverter<NullableDecimalConverter>();
            Map(m => m.High).Name("High").TypeConverter<NullableDecimalConverter>();
            Map(m => m.Low).Name("Low").TypeConverter<NullableDecimalConverter>();
            Map(m => m.Close).Name("Close").TypeConverter<NullableDecimalConverter>();
            Map(m => m.AdjClose).Name("Adj Close").TypeConverter<NullableDecimalConverter>();
            Map(m => m.Volume).Name("Volume").TypeConverter<NullableLongConverter>();
        }
    }
}
