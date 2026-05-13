using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addtableLabOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagnose_Doctors_doctorId",
                table: "Diagnose");

            migrationBuilder.DropForeignKey(
                name: "FK_Diagnose_Patients_PatientId",
                table: "Diagnose");

            migrationBuilder.DropForeignKey(
                name: "FK_Diagnose_TerminologyCodes_TerminologyCodeId",
                table: "Diagnose");

            migrationBuilder.DropForeignKey(
                name: "FK_LabImages_LabTestResult_LabTestResultId",
                table: "LabImages");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResult_Doctors_DoctorId",
                table: "LabTestResult");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResult_Encounters_EncounterId",
                table: "LabTestResult");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResult_Patients_PatientId",
                table: "LabTestResult");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResult_TerminologyCodes_TerminologyCodeId",
                table: "LabTestResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabTestResult",
                table: "LabTestResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Diagnose",
                table: "Diagnose");

            migrationBuilder.RenameTable(
                name: "LabTestResult",
                newName: "LabTestResults");

            migrationBuilder.RenameTable(
                name: "Diagnose",
                newName: "Diagnoses");

            migrationBuilder.RenameIndex(
                name: "IX_LabTestResult_TerminologyCodeId",
                table: "LabTestResults",
                newName: "IX_LabTestResults_TerminologyCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_LabTestResult_PatientId",
                table: "LabTestResults",
                newName: "IX_LabTestResults_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_LabTestResult_EncounterId",
                table: "LabTestResults",
                newName: "IX_LabTestResults_EncounterId");

            migrationBuilder.RenameIndex(
                name: "IX_LabTestResult_DoctorId",
                table: "LabTestResults",
                newName: "IX_LabTestResults_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnose_TerminologyCodeId",
                table: "Diagnoses",
                newName: "IX_Diagnoses_TerminologyCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnose_PatientId",
                table: "Diagnoses",
                newName: "IX_Diagnoses_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnose_doctorId",
                table: "Diagnoses",
                newName: "IX_Diagnoses_doctorId");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabTestResults",
                table: "LabTestResults",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Diagnoses",
                table: "Diagnoses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LabOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QrToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LabResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TerminologyCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabOrders_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabOrders_LabTestResults_LabResultId",
                        column: x => x.LabResultId,
                        principalTable: "LabTestResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabOrders_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabOrders_TerminologyCodes_TerminologyCodeId",
                        column: x => x.TerminologyCodeId,
                        principalTable: "TerminologyCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_DoctorId",
                table: "LabOrders",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_LabResultId",
                table: "LabOrders",
                column: "LabResultId",
                unique: true,
                filter: "[LabResultId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_PatientId",
                table: "LabOrders",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_TerminologyCodeId",
                table: "LabOrders",
                column: "TerminologyCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_Doctors_doctorId",
                table: "Diagnoses",
                column: "doctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_Patients_PatientId",
                table: "Diagnoses",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_TerminologyCodes_TerminologyCodeId",
                table: "Diagnoses",
                column: "TerminologyCodeId",
                principalTable: "TerminologyCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabImages_LabTestResults_LabTestResultId",
                table: "LabImages",
                column: "LabTestResultId",
                principalTable: "LabTestResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResults_Doctors_DoctorId",
                table: "LabTestResults",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResults_Encounters_EncounterId",
                table: "LabTestResults",
                column: "EncounterId",
                principalTable: "Encounters",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_Doctors_doctorId",
                table: "Diagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_Patients_PatientId",
                table: "Diagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_TerminologyCodes_TerminologyCodeId",
                table: "Diagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_LabImages_LabTestResults_LabTestResultId",
                table: "LabImages");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResults_Doctors_DoctorId",
                table: "LabTestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResults_Encounters_EncounterId",
                table: "LabTestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResults_Patients_PatientId",
                table: "LabTestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResults_TerminologyCodes_TerminologyCodeId",
                table: "LabTestResults");

            migrationBuilder.DropTable(
                name: "LabOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabTestResults",
                table: "LabTestResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Diagnoses",
                table: "Diagnoses");

            migrationBuilder.RenameTable(
                name: "LabTestResults",
                newName: "LabTestResult");

            migrationBuilder.RenameTable(
                name: "Diagnoses",
                newName: "Diagnose");

            migrationBuilder.RenameIndex(
                name: "IX_LabTestResults_TerminologyCodeId",
                table: "LabTestResult",
                newName: "IX_LabTestResult_TerminologyCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_LabTestResults_PatientId",
                table: "LabTestResult",
                newName: "IX_LabTestResult_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_LabTestResults_EncounterId",
                table: "LabTestResult",
                newName: "IX_LabTestResult_EncounterId");

            migrationBuilder.RenameIndex(
                name: "IX_LabTestResults_DoctorId",
                table: "LabTestResult",
                newName: "IX_LabTestResult_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnoses_TerminologyCodeId",
                table: "Diagnose",
                newName: "IX_Diagnose_TerminologyCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnoses_PatientId",
                table: "Diagnose",
                newName: "IX_Diagnose_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnoses_doctorId",
                table: "Diagnose",
                newName: "IX_Diagnose_doctorId");

            migrationBuilder.AlterColumn<decimal>(
                name: "value",
                table: "LabTestResult",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "LabTestResult",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabTestResult",
                table: "LabTestResult",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Diagnose",
                table: "Diagnose",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnose_Doctors_doctorId",
                table: "Diagnose",
                column: "doctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnose_Patients_PatientId",
                table: "Diagnose",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnose_TerminologyCodes_TerminologyCodeId",
                table: "Diagnose",
                column: "TerminologyCodeId",
                principalTable: "TerminologyCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabImages_LabTestResult_LabTestResultId",
                table: "LabImages",
                column: "LabTestResultId",
                principalTable: "LabTestResult",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResult_Doctors_DoctorId",
                table: "LabTestResult",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResult_Encounters_EncounterId",
                table: "LabTestResult",
                column: "EncounterId",
                principalTable: "Encounters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResult_Patients_PatientId",
                table: "LabTestResult",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResult_TerminologyCodes_TerminologyCodeId",
                table: "LabTestResult",
                column: "TerminologyCodeId",
                principalTable: "TerminologyCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
