using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GptFinance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStockExchangeFromCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_StockExchange_StockExchangeId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_StockExchangeId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "StockExchangeId",
                table: "Companies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StockExchangeId",
                table: "Companies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Companies_StockExchangeId",
                table: "Companies",
                column: "StockExchangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_StockExchange_StockExchangeId",
                table: "Companies",
                column: "StockExchangeId",
                principalTable: "StockExchange",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
