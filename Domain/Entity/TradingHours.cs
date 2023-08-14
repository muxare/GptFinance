namespace GptFinance.Domain.Entity
{
    public class TradingHours
    {
        public TimeSpan Open { get; set; }
        public TimeSpan Close { get; set; }

        public TradingHours()
        {
        }

        public TradingHours(TimeSpan open, TimeSpan close)
        {
            if (open >= close)
            {
                throw new ArgumentException("Open time must be earlier than close time.");
            }

            Open = open;
            Close = close;
        }

        // You may also include methods related to trading hours, such as:
        public bool IsTradingTime(TimeSpan currentTime)
        {
            return currentTime >= Open && currentTime <= Close;
        }

        // Override Equals and GetHashCode if you need to compare instances
    }
}
