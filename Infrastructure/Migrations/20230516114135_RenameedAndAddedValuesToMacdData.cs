using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GptFinance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameedAndAddedValuesToMacdData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "MacdData",
                newName: "SignalValue");

            migrationBuilder.AddColumn<decimal>(
                name: "HistogramValue",
                table: "MacdData",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MacdValue",
                table: "MacdData",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HistogramValue",
                table: "MacdData");

            migrationBuilder.DropColumn(
                name: "MacdValue",
                table: "MacdData");

            migrationBuilder.RenameColumn(
                name: "SignalValue",
                table: "MacdData",
                newName: "Value");
        }
    }
}
