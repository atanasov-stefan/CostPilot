using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CostPilot.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCostRequestEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CostRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Cost Request Unique Identifier"),
                    Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, comment: "Cost Request Number"),
                    Amount = table.Column<decimal>(type: "decimal(30,2)", nullable: false, comment: "Cost Request Amount"),
                    SubmittedOn = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Cost Request SubmittedOn Date"),
                    DecisionOn = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Cost Request DecisionOn Date"),
                    RequestorId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "Foreign Key Reference To Application User - Requestor"),
                    ApproverId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "Foreign Key Reference To Application User - Approver"),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Cost Request Approver Comment"),
                    BriefDescription = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Cost Request Brief Description"),
                    DetailedDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Cost Request Detailed Description"),
                    CenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign Key Reference To Cost Center"),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign Key Reference To Cost Currency"),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign Key Reference To Cost Status"),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign Key Reference To Cost Type"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Cost Request IsDeleted Indicator")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostRequests_AspNetUsers_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostRequests_AspNetUsers_RequestorId",
                        column: x => x.RequestorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostRequests_CostCenters_CenterId",
                        column: x => x.CenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostRequests_CostCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CostCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostRequests_CostStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "CostStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostRequests_CostTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "CostTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostRequests_ApproverId",
                table: "CostRequests",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_CostRequests_CenterId",
                table: "CostRequests",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_CostRequests_CurrencyId",
                table: "CostRequests",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CostRequests_RequestorId",
                table: "CostRequests",
                column: "RequestorId");

            migrationBuilder.CreateIndex(
                name: "IX_CostRequests_StatusId",
                table: "CostRequests",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CostRequests_TypeId",
                table: "CostRequests",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostRequests");
        }
    }
}
