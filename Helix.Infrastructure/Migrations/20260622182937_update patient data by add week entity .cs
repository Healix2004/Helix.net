using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatepatientdatabyaddweekentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyContact_Patients_PatientId",
                table: "EmergencyContact");

            migrationBuilder.DropColumn(
                name: "ChronicDiseases",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PastSurgeries",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "name",
                table: "Allergies");

            migrationBuilder.RenameColumn(
                name: "InsuranceProvider",
                table: "Patients",
                newName: "Insurance_InsuranceProvider");

            migrationBuilder.RenameColumn(
                name: "InsurancePolicyNumber",
                table: "Patients",
                newName: "Insurance_InsurancePolicyNumber");

            migrationBuilder.RenameColumn(
                name: "criticality",
                table: "Allergies",
                newName: "Criticality");

            migrationBuilder.AddColumn<string>(
                name: "Surgeries",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PatientId",
                table: "EmergencyContact",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Criticality",
                table: "Allergies",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "Allergies",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AllergenCatalogCode",
                table: "Allergies",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ClinicalStatus",
                table: "Allergies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Reaction",
                table: "Allergies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Severity",
                table: "Allergies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChronicDisease",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiagnosisDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChronicDisease", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChronicDisease_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Allergies_AllergenCatalogCode",
                table: "Allergies",
                column: "AllergenCatalogCode");

            migrationBuilder.CreateIndex(
                name: "IX_ChronicDisease_PatientId",
                table: "ChronicDisease",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Allergies_AllergenCatalogs_AllergenCatalogCode",
                table: "Allergies",
                column: "AllergenCatalogCode",
                principalTable: "AllergenCatalogs",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyContact_Patients_PatientId",
                table: "EmergencyContact",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allergies_AllergenCatalogs_AllergenCatalogCode",
                table: "Allergies");

            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyContact_Patients_PatientId",
                table: "EmergencyContact");

            migrationBuilder.DropTable(
                name: "ChronicDisease");

            migrationBuilder.DropIndex(
                name: "IX_Allergies_AllergenCatalogCode",
                table: "Allergies");

            migrationBuilder.DropColumn(
                name: "Surgeries",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "AllergenCatalogCode",
                table: "Allergies");

            migrationBuilder.DropColumn(
                name: "ClinicalStatus",
                table: "Allergies");

            migrationBuilder.DropColumn(
                name: "Reaction",
                table: "Allergies");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Allergies");

            migrationBuilder.RenameColumn(
                name: "Insurance_InsuranceProvider",
                table: "Patients",
                newName: "InsuranceProvider");

            migrationBuilder.RenameColumn(
                name: "Insurance_InsurancePolicyNumber",
                table: "Patients",
                newName: "InsurancePolicyNumber");

            migrationBuilder.RenameColumn(
                name: "Criticality",
                table: "Allergies",
                newName: "criticality");

            migrationBuilder.AddColumn<string>(
                name: "ChronicDiseases",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PastSurgeries",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "PatientId",
                table: "EmergencyContact",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "criticality",
                table: "Allergies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Allergies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "Allergies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyContact_Patients_PatientId",
                table: "EmergencyContact",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }
    }
}
