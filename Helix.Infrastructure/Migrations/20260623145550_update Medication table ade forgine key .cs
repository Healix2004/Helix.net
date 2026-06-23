using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateMedicationtableadeforginekey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medication_MedicationCatalogs_medicationCatalogRxcui",
                table: "Medication");

            migrationBuilder.AlterColumn<string>(
                name: "medicationCatalogRxcui",
                table: "Medication",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Medication_MedicationCatalogs_medicationCatalogRxcui",
                table: "Medication",
                column: "medicationCatalogRxcui",
                principalTable: "MedicationCatalogs",
                principalColumn: "Rxcui",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medication_MedicationCatalogs_medicationCatalogRxcui",
                table: "Medication");

            migrationBuilder.AlterColumn<string>(
                name: "medicationCatalogRxcui",
                table: "Medication",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            migrationBuilder.AddForeignKey(
                name: "FK_Medication_MedicationCatalogs_medicationCatalogRxcui",
                table: "Medication",
                column: "medicationCatalogRxcui",
                principalTable: "MedicationCatalogs",
                principalColumn: "Rxcui");
        }
    }
}
