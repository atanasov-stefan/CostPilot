using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CostPilot.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCostTypeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CostTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Cost Type Unique Identifier"),
                    Code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false, comment: "Cost Type Code"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Cost Type Description"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Cost Type IsDeleted Indicator")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CostTypes",
                columns: new[] { "Id", "Code", "Description", "IsDeleted" },
                values: new object[,]
                {
                    { new Guid("19befbf8-9a67-4aca-9ee1-92bd7b300324"), "SI", "Sustaining Investment", false },
                    { new Guid("4936a809-ee57-43a8-842c-2144457e1b90"), "IP", "Investment Project", false },
                    { new Guid("c2bf674f-65c9-4865-ac0f-5b5eb8ab64d7"), "OC", "Operating Costs", false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostTypes");
        }
    }
}
