using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GptFinance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmaData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Period = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmaData", x => new { x.Id, x.CompanyId, x.Date });
                });

            migrationBuilder.CreateTable(
                name: "MacdData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MacdValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SignalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HistogramValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShortPeriod = table.Column<int>(type: "int", nullable: false),
                    LongPeriod = table.Column<int>(type: "int", nullable: false),
                    SignalPeriod = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MacdData", x => new { x.Id, x.CompanyId, x.Date });
                });

            migrationBuilder.CreateTable(
                name: "StockExchange",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Marketplace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ranking = table.Column<int>(type: "int", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TradingHours_Open = table.Column<TimeSpan>(type: "time", nullable: false),
                    TradingHours_Close = table.Column<TimeSpan>(type: "time", nullable: false),
                    LunchBreak_Start = table.Column<TimeSpan>(type: "time", nullable: false),
                    LunchBreak_End = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockExchange", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StockExchangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_StockExchange_StockExchangeId",
                        column: x => x.StockExchangeId,
                        principalTable: "StockExchange",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EodData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    High = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Low = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Close = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Volume = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EodData", x => new { x.Id, x.CompanyId, x.Date });
                    table.ForeignKey(
                        name: "FK_EodData_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_StockExchangeId",
                table: "Companies",
                column: "StockExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_EodData_CompanyId",
                table: "EodData",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmaData");

            migrationBuilder.DropTable(
                name: "EodData");

            migrationBuilder.DropTable(
                name: "MacdData");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "StockExchange");
        }
    }
}
