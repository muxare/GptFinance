namespace GptFinance.Application.Models.Dtos;

public class BaseDto
{
    public Guid Id { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class CompanyDto : BaseDto
{
    public string? Symbol { get; set; }
    public string? Name { get; set; }
}


