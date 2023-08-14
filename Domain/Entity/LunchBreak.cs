namespace GptFinance.Domain.Entity
{
    public class LunchBreak
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public LunchBreak()
        {
        }

        public LunchBreak(TimeSpan start, TimeSpan end)
        {
            if (start >= end)
            {
                throw new ArgumentException("Start time must be earlier than end time.");
            }

            Start = start;
            End = end;
        }

        // Additional methods and overrides as needed
    }
}
