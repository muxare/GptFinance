namespace YahooFinanceAPI.Services
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;

    public class NullableDecimalConverter : DecimalConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text) || text.Trim().Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return base.ConvertFromString(text, row, memberMapData);
        }
    }
}
