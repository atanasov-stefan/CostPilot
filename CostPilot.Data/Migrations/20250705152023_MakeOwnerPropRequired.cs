using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CostPilot.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeOwnerPropRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "CostCenters",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                comment: "Foreign Key Reference To Application User",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true,
                oldComment: "Foreign Key Reference To Application User");

            migrationBuilder.UpdateData(
                table: "CostCenters",
                keyColumn: "Id",
                keyValue: new Guid("28afb175-cbad-4f83-8a6c-54eba351497e"),
                column: "OwnerId",
                value: "e977ec1a-c3ab-4915-9047-433a315cd635");

            migrationBuilder.UpdateData(
                table: "CostCenters",
                keyColumn: "Id",
                keyValue: new Guid("55cf3a59-c1f8-4d57-9d0c-d99c9b58f50e"),
                column: "OwnerId",
                value: "e977ec1a-c3ab-4915-9047-433a315cd635");

            migrationBuilder.UpdateData(
                table: "CostCenters",
                keyColumn: "Id",
                keyValue: new Guid("59376db9-4aa1-4aa2-aabf-fad94933df13"),
                column: "OwnerId",
                value: "e977ec1a-c3ab-4915-9047-433a315cd635");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "CostCenters",
                type: "nvarchar(450)",
                nullable: true,
                comment: "Foreign Key Reference To Application User",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComment: "Foreign Key Reference To Application User");

            migrationBuilder.UpdateData(
                table: "CostCenters",
                keyColumn: "Id",
                keyValue: new Guid("28afb175-cbad-4f83-8a6c-54eba351497e"),
                column: "OwnerId",
                value: null);

            migrationBuilder.UpdateData(
                table: "CostCenters",
                keyColumn: "Id",
                keyValue: new Guid("55cf3a59-c1f8-4d57-9d0c-d99c9b58f50e"),
                column: "OwnerId",
                value: null);

            migrationBuilder.UpdateData(
                table: "CostCenters",
                keyColumn: "Id",
                keyValue: new Guid("59376db9-4aa1-4aa2-aabf-fad94933df13"),
                column: "OwnerId",
                value: null);
        }
    }
}
