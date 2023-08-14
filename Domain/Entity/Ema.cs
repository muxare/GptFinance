namespace GptFinance.Domain.Entity
{
    public record EmaDomainEntity(DateTime Date, int Window, decimal? Value);
    public record EmaFanDimainEntity(DateTime Date, decimal w18, decimal w50, decimal w100, decimal w200);
}
