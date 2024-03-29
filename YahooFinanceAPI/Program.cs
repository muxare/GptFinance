using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using GptFinance.Application.Interfaces;
using GptFinance.Infrastructure.Data;
using GptFinance.Infrastructure.Models;
using GptFinance.Infrastructure.Services;
using GptFinance.Infrastructure.Repository;
using GptFinance.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddLogging(logBuilder => logBuilder.AddApplicationInsights());

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IYahooFinanceService<CsvRecord>, YahooFinanceService>();
builder.Services.AddTransient<IEodDataRepository, EodDataRepository>();
//builder.Services.AddTransient<IStockExchangeRepository, StockExchangeRepository>();
builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();
builder.Services.AddTransient<ITechnicalIndicatorsService, TechnicalIndicatorsService>();
builder.Services.AddTransient<IYahooSearchService, YahooSearchService>();
builder.Services.AddTransient<IEmaRepository, EmaRepository>();
builder.Services.AddTransient<IMacdRepository, MacdRepository>();
builder.Services.AddTransient<ICompanyScreenerService, CompanyScreenerService>();

builder.Services.AddTransient<ICompanyService, CompanyService>();

builder.Services.AddSingleton<HttpClient>();

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
