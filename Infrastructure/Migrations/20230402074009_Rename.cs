using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GptFinance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EMAData_Companies_CompanyId",
                table: "EMAData");

            migrationBuilder.DropForeignKey(
                name: "FK_EODData_Companies_CompanyId",
                table: "EODData");

            migrationBuilder.DropForeignKey(
                name: "FK_MACDData_Companies_CompanyId",
                table: "MACDData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MACDData",
                table: "MACDData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EODData",
                table: "EODData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EMAData",
                table: "EMAData");

            migrationBuilder.RenameTable(
                name: "MACDData",
                newName: "MacdData");

            migrationBuilder.RenameTable(
                name: "EODData",
                newName: "EodData");

            migrationBuilder.RenameTable(
                name: "EMAData",
                newName: "EmaData");

            migrationBuilder.RenameIndex(
                name: "IX_MACDData_CompanyId",
                table: "MacdData",
                newName: "IX_MacdData_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_EODData_CompanyId",
                table: "EodData",
                newName: "IX_EodData_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_EMAData_CompanyId",
                table: "EmaData",
                newName: "IX_EmaData_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MacdData",
                table: "MacdData",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EodData",
                table: "EodData",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmaData",
                table: "EmaData",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmaData_Companies_CompanyId",
                table: "EmaData",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EodData_Companies_CompanyId",
                table: "EodData",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MacdData_Companies_CompanyId",
                table: "MacdData",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmaData_Companies_CompanyId",
                table: "EmaData");

            migrationBuilder.DropForeignKey(
                name: "FK_EodData_Companies_CompanyId",
                table: "EodData");

            migrationBuilder.DropForeignKey(
                name: "FK_MacdData_Companies_CompanyId",
                table: "MacdData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MacdData",
                table: "MacdData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EodData",
                table: "EodData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmaData",
                table: "EmaData");

            migrationBuilder.RenameTable(
                name: "MacdData",
                newName: "MACDData");

            migrationBuilder.RenameTable(
                name: "EodData",
                newName: "EODData");

            migrationBuilder.RenameTable(
                name: "EmaData",
                newName: "EMAData");

            migrationBuilder.RenameIndex(
                name: "IX_MacdData_CompanyId",
                table: "MACDData",
                newName: "IX_MACDData_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_EodData_CompanyId",
                table: "EODData",
                newName: "IX_EODData_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_EmaData_CompanyId",
                table: "EMAData",
                newName: "IX_EMAData_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MACDData",
                table: "MACDData",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EODData",
                table: "EODData",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EMAData",
                table: "EMAData",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EMAData_Companies_CompanyId",
                table: "EMAData",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EODData_Companies_CompanyId",
                table: "EODData",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MACDData_Companies_CompanyId",
                table: "MACDData",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
