namespace GptFinance.Domain.Entity
{
    public record Ema(DateTime Date, int Window, decimal Value);
}
