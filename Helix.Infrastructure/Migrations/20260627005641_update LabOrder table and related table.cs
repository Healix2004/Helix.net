using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateLabOrdertableandrelatedtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabOrders_LabTestResults_LabResultId",
                table: "LabOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_LabOrders_TerminologyCodes_TerminologyCodeId",
                table: "LabOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResults_Patients_PatientId",
                table: "LabTestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResults_TerminologyCodes_TerminologyCodeId",
                table: "LabTestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyResults_Patients_PatientId",
                table: "RadiologyResults");

            migrationBuilder.DropIndex(
                name: "IX_RadiologyResults_PatientId",
                table: "RadiologyResults");

            migrationBuilder.DropIndex(
                name: "IX_LabOrders_LabResultId",
                table: "LabOrders");

            migrationBuilder.DropColumn(
                name: "LabResultId",
                table: "LabOrders");

            migrationBuilder.RenameColumn(
                name: "TerminologyCodeId",
                table: "LabTestResults",
                newName: "MedicalConceptId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "LabTestResults",
                newName: "LabOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_LabTestResults_TerminologyCodeId",
                table: "LabTestResults",
                newName: "IX_LabTestResults_MedicalConceptId");

            migrationBuilder.RenameColumn(
                name: "TerminologyCodeId",
                table: "LabOrders",
                newName: "MedicalConceptId");

            migrationBuilder.RenameIndex(
                name: "IX_LabOrders_TerminologyCodeId",
                table: "LabOrders",
                newName: "IX_LabOrders_MedicalConceptId");

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "LabTestResults",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_LabTestResults_LabOrderId",
                table: "LabTestResults",
                column: "LabOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrders_MedicalConceptCatalogs_MedicalConceptId",
                table: "LabOrders",
                column: "MedicalConceptId",
                principalTable: "MedicalConceptCatalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResults_LabOrders_LabOrderId",
                table: "LabTestResults",
                column: "LabOrderId",
                principalTable: "LabOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResults_MedicalConceptCatalogs_MedicalConceptId",
                table: "LabTestResults",
                column: "MedicalConceptId",
                principalTable: "MedicalConceptCatalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResults_Patients_PatientId",
                table: "LabTestResults",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabOrders_MedicalConceptCatalogs_MedicalConceptId",
                table: "LabOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResults_LabOrders_LabOrderId",
                table: "LabTestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResults_MedicalConceptCatalogs_MedicalConceptId",
                table: "LabTestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResults_Patients_PatientId",
                table: "LabTestResults");

            migrationBuilder.DropIndex(
                name: "IX_LabTestResults_LabOrderId",
                table: "LabTestResults");

            migrationBuilder.RenameColumn(
                name: "MedicalConceptId",
                table: "LabTestResults",
                newName: "TerminologyCodeId");

            migrationBuilder.RenameColumn(
                name: "LabOrderId",
                table: "LabTestResults",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_LabTestResults_MedicalConceptId",
                table: "LabTestResults",
                newName: "IX_LabTestResults_TerminologyCodeId");

            migrationBuilder.RenameColumn(
                name: "MedicalConceptId",
                table: "LabOrders",
                newName: "TerminologyCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_LabOrders_MedicalConceptId",
                table: "LabOrders",
                newName: "IX_LabOrders_TerminologyCodeId");

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

            migrationBuilder.AddColumn<Guid>(
                name: "LabResultId",
                table: "LabOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyResults_PatientId",
                table: "RadiologyResults",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_LabResultId",
                table: "LabOrders",
                column: "LabResultId",
                unique: true,
                filter: "[LabResultId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrders_LabTestResults_LabResultId",
                table: "LabOrders",
                column: "LabResultId",
                principalTable: "LabTestResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrders_TerminologyCodes_TerminologyCodeId",
                table: "LabOrders",
                column: "TerminologyCodeId",
                principalTable: "TerminologyCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResults_Patients_PatientId",
                table: "LabTestResults",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResults_TerminologyCodes_TerminologyCodeId",
                table: "LabTestResults",
                column: "TerminologyCodeId",
                principalTable: "TerminologyCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyResults_Patients_PatientId",
                table: "RadiologyResults",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
