using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addtableMedicationCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Surgeries",
                table: "Patients");

            migrationBuilder.CreateTable(
                name: "MedicationCatalogs",
                columns: table => new
                {
                    Rxcui = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DrugName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TermType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationCatalogs", x => x.Rxcui);
                });

            migrationBuilder.CreateTable(
                name: "Medication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    medicationCatalogRxcui = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medication_MedicationCatalogs_medicationCatalogRxcui",
                        column: x => x.medicationCatalogRxcui,
                        principalTable: "MedicationCatalogs",
                        principalColumn: "Rxcui");
                    table.ForeignKey(
                        name: "FK_Medication_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medication_medicationCatalogRxcui",
                table: "Medication",
                column: "medicationCatalogRxcui");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_PatientId",
                table: "Medication",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medication");

            migrationBuilder.DropTable(
                name: "MedicationCatalogs");

            migrationBuilder.AddColumn<string>(
                name: "Surgeries",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
