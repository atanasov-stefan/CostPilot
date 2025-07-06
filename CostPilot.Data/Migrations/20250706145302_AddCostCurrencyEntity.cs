using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CostPilot.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCostCurrencyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CostCurrencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Cost Currency Unique Identifier"),
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, comment: "Cost Currency Code"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Cost Currency IsDeleted Indicator")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCurrencies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CostCurrencies",
                columns: new[] { "Id", "Code", "IsDeleted" },
                values: new object[,]
                {
                    { new Guid("2eb6571e-2efe-4076-b9a8-ffcaaf6658dc"), "USD", false },
                    { new Guid("8307fe38-981d-4322-821e-e805353fd152"), "BGN", false },
                    { new Guid("a1eb3603-83fc-4c89-9e95-e0a73cd4c3ec"), "EUR", false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostCurrencies");
        }
    }
}
