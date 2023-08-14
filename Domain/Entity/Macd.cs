namespace GptFinance.Domain.Entity
{
    public record MacdDomainEntity(DateTime Date, int FastWindow, int SlowWindow, int SignalWindow, decimal? MacdValue, decimal? SignalValue, decimal? HistogramValue);
}
