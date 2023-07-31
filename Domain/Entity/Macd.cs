namespace GptFinance.Domain.Entity
{
    public record Macd(DateTime Date, int FastWindow, int SlowWindow, int SignalWindow, decimal MacdValue, decimal SignalValue, decimal HistogramValue);
}
