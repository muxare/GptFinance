using GptFinance.Domain.Entity;

namespace GptFinance.Domain.Aggregate
{
    public class FinancialDataAggregate
    {
        public ICollection<EodDomainEntity> EodData { get; set; } = new List<EodDomainEntity>();
        public ICollection<EmaDomainEntity> EmaData { get; set; } = new List<EmaDomainEntity>();
        public ICollection<MacdDomainEntity> MacdData { get; set; } = new List<MacdDomainEntity>();
        public ICollection<EmaFanDimainEntity> EmaFanData { get; set; } = new List<EmaFanDimainEntity>();
    }

    public class TrendAnalyzer
    {
        // Number of days to look back for EOD analysis
        private const int EodLookbackDays = 5;

        public ICollection<LabeledTrendEntity> AnalyzeTrends(
            ICollection<EmaDomainEntity> emaValues,
            ICollection<EodDomainEntity> eodValues,
            ICollection<MacdDomainEntity> macdValues)
        {
            var labeledTrends = new List<LabeledTrendEntity>();

            // Group the EMA values by date and create dictionaries for EOD and MACD data
            var emaGroupedByDate = emaValues.GroupBy(e => e.Date).ToDictionary(g => g.Key, g => g.ToDictionary(e => e.Window, e => e.Value));
            var eodByDate = eodValues.OrderBy(e => e.Date).ToList();
            var macdByDate = macdValues.ToDictionary(e => e.Date);

            for (int i = EodLookbackDays; i < eodByDate.Count; i++)
            {
                var date = eodByDate[i].Date;

                // Retrieve the EMA values for the specific windows
                if (!emaGroupedByDate.TryGetValue(date, out var emaValuesByWindow)) continue;
                var w18 = emaValuesByWindow.GetValueOrDefault(18);
                var w50 = emaValuesByWindow.GetValueOrDefault(50);
                var w100 = emaValuesByWindow.GetValueOrDefault(100);
                var w200 = emaValuesByWindow.GetValueOrDefault(200);

                // Determine the EMA trend label
                string emaLabel;
                if (w18 > w50 && w50 > w100 && w100 > w200)
                {
                    emaLabel = "Uptrend";
                }
                else if (w18 < w50 && w50 > w100 && w100 > w200)
                {
                    // Check the recent closing prices to see if they are consistently increasing
                    bool isIncreasingTrend = true;
                    for (int j = 1; j <= EodLookbackDays; j++)
                    {
                        if (eodByDate[i - j].Close <= eodByDate[i - j + 1].Close)
                        {
                            isIncreasingTrend = false;
                            break;
                        }
                    }

                    emaLabel = isIncreasingTrend ? "Probable Uptrend" : "Possible Uptrend";
                }
                else if (w18 < w50 && w50 < w100 && w100 < w200)
                {
                    emaLabel = "Downtrend";
                }
                else
                {
                    emaLabel = "Mixed Trend";
                }

                // Determine the MACD trend label (if MACD data is available)
                string macdLabel = "N/A";
                if (macdByDate.TryGetValue(date, out var macd))
                {
                    if (macd.MacdValue > macd.SignalValue)
                    {
                        macdLabel = "Bullish";
                    }
                    else if (macd.MacdValue < macd.SignalValue)
                    {
                        macdLabel = "Bearish";
                    }
                    else
                    {
                        macdLabel = "Neutral";
                    }
                }

                labeledTrends.Add(new LabeledTrendEntity(date, emaLabel, macdLabel));
            }

            return labeledTrends;
        }
    }
}
