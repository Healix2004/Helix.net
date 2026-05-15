using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addforginkeysforlabresluttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResults_Encounters_EncounterId",
                table: "LabTestResults");

            migrationBuilder.DropTable(
                name: "LabImages");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "LabTestResults",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "LabTestResults",
                newName: "Status");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "LabTestResults",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "LabTestResults",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EncounterId",
                table: "LabTestResults",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyCodes_Code",
                table: "TerminologyCodes",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResults_Encounters_EncounterId",
                table: "LabTestResults",
                column: "EncounterId",
                principalTable: "Encounters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResults_Encounters_EncounterId",
                table: "LabTestResults");

            migrationBuilder.DropIndex(
                name: "IX_TerminologyCodes_Code",
                table: "TerminologyCodes");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "LabTestResults",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "LabTestResults",
                newName: "status");

            migrationBuilder.AlterColumn<decimal>(
                name: "value",
                table: "LabTestResults",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "LabTestResults",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "EncounterId",
                table: "LabTestResults",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "LabImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabTestResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabImages_LabTestResults_LabTestResultId",
                        column: x => x.LabTestResultId,
                        principalTable: "LabTestResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabImages_LabTestResultId",
                table: "LabImages",
                column: "LabTestResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResults_Encounters_EncounterId",
                table: "LabTestResults",
                column: "EncounterId",
                principalTable: "Encounters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
