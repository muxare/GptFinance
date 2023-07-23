namespace GptFinance.Application.Models.Yahoo;

public class News
{
    public string uuid { get; set; }
    public string title { get; set; }
    public string publisher { get; set; }
    public string link { get; set; }
    public int providerPublishTime { get; set; }
    public string type { get; set; }
    public Thumbnail thumbnail { get; set; }
    public List<string> relatedTickers { get; set; }
}
