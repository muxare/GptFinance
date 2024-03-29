﻿using GptFinance.Application.Models.Dto;
using GptFinance.Application.Models.Yahoo;

namespace GptFinance.Application.Models;

public static class Extentions
{
    public static YahooCompanySearchResult ToYahooCompanySearchResult(this List<SearchResult> searchResults)
    {
        var yahooCompanySearchResult = new YahooCompanySearchResult
        {
            Companies = searchResults.SelectMany(sr => sr.quotes.ToCompanySearchDto()).ToList()
        };

        return yahooCompanySearchResult;
    }

    public static YahooCompanySearchResult ToYahooCompanySearchResult(this SearchResult searchResult)
    {
        var yahooCompanySearchResult = new YahooCompanySearchResult
        {
            Companies = searchResult.quotes.ToCompanySearchDto()
        };

        return yahooCompanySearchResult;
    }

    public static List<CompanySearchDto> ToCompanySearchDto(this List<Quote> quote)
    {
        return quote.Select(q => q.ToCompanySearchDto()).ToList();
    }

    public static CompanySearchDto ToCompanySearchDto(this Quote quote)
    {
        return new CompanySearchDto()
        {
            ExchangeName = quote.exchange,
            ShortName = quote.shortname,
            QuoteType = quote.quoteType,
            Symbol = quote.symbol,
            Index = quote.index,
            TypeDisp = quote.typeDisp,
            LongName = quote.longname,
            ExchDisp = quote.exchDisp,
            Sector= quote.sector,
            Industry= quote.industry,
            DispSecIndFlag = quote.dispSecIndFlag,
            IsYahooFinance = quote.isYahooFinance
        };
    }
}
