using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRadiologyModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyOrders_TerminologyCodes_TerminologyCodeId",
                table: "RadiologyOrders");

            migrationBuilder.DropIndex(
                name: "IX_RadiologyResults_OrderId",
                table: "RadiologyResults");

            migrationBuilder.RenameColumn(
                name: "TerminologyCodeId",
                table: "RadiologyOrders",
                newName: "MedicalConceptId");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "RadiologyOrders",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_RadiologyOrders_TerminologyCodeId",
                table: "RadiologyOrders",
                newName: "IX_RadiologyOrders_MedicalConceptId");

            migrationBuilder.AddColumn<string>(
                name: "ReasonForExam",
                table: "RadiologyOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RadiologyReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RadiologyOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalRadiologistName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Findings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Conclusion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DicomStudyInstanceUid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyReport_RadiologyOrders_RadiologyOrderId",
                        column: x => x.RadiologyOrderId,
                        principalTable: "RadiologyOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyResults_OrderId",
                table: "RadiologyResults",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyReport_RadiologyOrderId",
                table: "RadiologyReport",
                column: "RadiologyOrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyOrders_MedicalConceptCatalogs_MedicalConceptId",
                table: "RadiologyOrders",
                column: "MedicalConceptId",
                principalTable: "MedicalConceptCatalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyOrders_MedicalConceptCatalogs_MedicalConceptId",
                table: "RadiologyOrders");

            migrationBuilder.DropTable(
                name: "RadiologyReport");

            migrationBuilder.DropIndex(
                name: "IX_RadiologyResults_OrderId",
                table: "RadiologyResults");

            migrationBuilder.DropColumn(
                name: "ReasonForExam",
                table: "RadiologyOrders");

            migrationBuilder.RenameColumn(
                name: "MedicalConceptId",
                table: "RadiologyOrders",
                newName: "TerminologyCodeId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "RadiologyOrders",
                newName: "CreateDate");

            migrationBuilder.RenameIndex(
                name: "IX_RadiologyOrders_MedicalConceptId",
                table: "RadiologyOrders",
                newName: "IX_RadiologyOrders_TerminologyCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyResults_OrderId",
                table: "RadiologyResults",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyOrders_TerminologyCodes_TerminologyCodeId",
                table: "RadiologyOrders",
                column: "TerminologyCodeId",
                principalTable: "TerminologyCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
