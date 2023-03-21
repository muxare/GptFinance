namespace YahooFinanceAPI.Services
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using System.Globalization;

    public class NullableLongConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out long result))
            {
                return result;
            }

            return base.ConvertFromString(text, row, memberMapData);
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }
    }
}
