namespace GptFinance.Domain.Entities
{
    // Models/EMAData.cs
    public class EmaData
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public int Period { get; set; }
    }
}
