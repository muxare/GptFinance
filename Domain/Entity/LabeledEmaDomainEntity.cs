namespace GptFinance.Domain.Entity
{
    public record LabeledTrendEntity(DateTime Date, string EmaLabel, string MacdLabel);
}
