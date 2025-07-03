using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CostPilot.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCostStatusEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CostStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Cost Status Unique Identifier"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Cost Status Description"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Cost Status IsDeleted Indicator")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostStatuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CostStatuses",
                columns: new[] { "Id", "Description", "IsDeleted" },
                values: new object[,]
                {
                    { new Guid("6abfacab-495a-4b43-bca3-b2ba53c71d9a"), "Approved", false },
                    { new Guid("c584c84f-fcd2-464a-957e-cfd57549ccaa"), "Pending", false },
                    { new Guid("f53b3520-6ebd-4d00-b280-a109ac0a0b54"), "Rejected", false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostStatuses");
        }
    }
}
