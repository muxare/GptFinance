namespace GptFinance.Application.Models.Yahoo;

public class SearchResult
{
    public List<object> explains { get; set; }
    public int count { get; set; }
    public List<Quote> quotes { get; set; }
    public List<News> news { get; set; }
}
