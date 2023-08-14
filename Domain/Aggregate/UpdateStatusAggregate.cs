namespace GptFinance.Domain.Aggregate
{
    public class UpdateStatusAggregate
    {
        public DateTime EodLatestDateTime { get; set; }
        public int EodCount { get; set; }
    }
}
