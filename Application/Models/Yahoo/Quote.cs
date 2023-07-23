namespace GptFinance.Application.Models.Yahoo;

public class Quote
{
    public string exchange { get; set; }
    public string shortname { get; set; }
    public string quoteType { get; set; }
    public string symbol { get; set; }
    public string index { get; set; }
    public double score { get; set; }
    public string typeDisp { get; set; }
    public string longname { get; set; }
    public string exchDisp { get; set; }
    public string sector { get; set; }
    public string industry { get; set; }
    public bool dispSecIndFlag { get; set; }
    public bool isYahooFinance { get; set; }
}
