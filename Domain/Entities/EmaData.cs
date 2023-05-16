namespace GptFinance.Domain.Entities
{
    // Models/EMAData.cs
    public class EmaData
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public int Period { get; set; }
    }
}
