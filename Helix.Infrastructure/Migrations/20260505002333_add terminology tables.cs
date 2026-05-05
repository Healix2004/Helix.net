using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addterminologytables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalConcepts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabImages",
                table: "LabImages");

            migrationBuilder.DropColumn(
                name: "name",
                table: "LabTestResult");

            migrationBuilder.RenameColumn(
                name: "Specialization",
                table: "Doctors",
                newName: "SyndicateNumber");

            migrationBuilder.AddColumn<int>(
                name: "BloodType",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "LabTestResult",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "LabTestResult",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "LabTestResult",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TerminologyCodeId",
                table: "LabTestResult",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "LabTestResult",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "LabImages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "LabImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Doctors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ConsultationFee",
                table: "Doctors",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Specialty",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabImages",
                table: "LabImages",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SubscriptionPlan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FacilitieType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TerminologyCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Display = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SystemUrl = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TerminologyType = table.Column<int>(type: "int", nullable: false),
                    UsageCount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminologyCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoctorFacilitie",
                columns: table => new
                {
                    DoctorsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorFacilitie", x => new { x.DoctorsId, x.FacilitiesId });
                    table.ForeignKey(
                        name: "FK_DoctorFacilitie_Doctors_DoctorsId",
                        column: x => x.DoctorsId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorFacilitie_Facilities_FacilitiesId",
                        column: x => x.FacilitiesId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Diagnose",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TerminologyCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    doctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnose", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diagnose_Doctors_doctorId",
                        column: x => x.doctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Diagnose_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Diagnose_TerminologyCodes_TerminologyCodeId",
                        column: x => x.TerminologyCodeId,
                        principalTable: "TerminologyCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabTestResult_DoctorId",
                table: "LabTestResult",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestResult_PatientId",
                table: "LabTestResult",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestResult_TerminologyCodeId",
                table: "LabTestResult",
                column: "TerminologyCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_LabImages_LabTestResultId",
                table: "LabImages",
                column: "LabTestResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnose_doctorId",
                table: "Diagnose",
                column: "doctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnose_PatientId",
                table: "Diagnose",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnose_TerminologyCodeId",
                table: "Diagnose",
                column: "TerminologyCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorFacilitie_FacilitiesId",
                table: "DoctorFacilitie",
                column: "FacilitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyCodes_Display",
                table: "TerminologyCodes",
                column: "Display");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyCodes_Display_SystemUrl",
                table: "TerminologyCodes",
                columns: new[] { "Display", "SystemUrl" });

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResult_Doctors_DoctorId",
                table: "LabTestResult",
                column: "DoctorId",
                principalTable: "Doctors",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResult_Doctors_DoctorId",
                table: "LabTestResult");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResult_Patients_PatientId",
                table: "LabTestResult");

            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResult_TerminologyCodes_TerminologyCodeId",
                table: "LabTestResult");

            migrationBuilder.DropTable(
                name: "Diagnose");

            migrationBuilder.DropTable(
                name: "DoctorFacilitie");

            migrationBuilder.DropTable(
                name: "TerminologyCodes");

            migrationBuilder.DropTable(
                name: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_LabTestResult_DoctorId",
                table: "LabTestResult");

            migrationBuilder.DropIndex(
                name: "IX_LabTestResult_PatientId",
                table: "LabTestResult");

            migrationBuilder.DropIndex(
                name: "IX_LabTestResult_TerminologyCodeId",
                table: "LabTestResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabImages",
                table: "LabImages");

            migrationBuilder.DropIndex(
                name: "IX_LabImages_LabTestResultId",
                table: "LabImages");

            migrationBuilder.DropColumn(
                name: "BloodType",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "LabTestResult");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "LabTestResult");

            migrationBuilder.DropColumn(
                name: "TerminologyCodeId",
                table: "LabTestResult");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "LabTestResult");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LabImages");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ConsultationFee",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Specialty",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "SyndicateNumber",
                table: "Doctors",
                newName: "Specialization");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "LabTestResult",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "LabTestResult",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "LabImages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabImages",
                table: "LabImages",
                columns: new[] { "LabTestResultId", "ImageUrl" });

            migrationBuilder.CreateTable(
                name: "MedicalConcepts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Display = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PropertiesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SystemUri = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalConcepts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalConcepts_Display",
                table: "MedicalConcepts",
                column: "Display");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalConcepts_SystemUri_Code",
                table: "MedicalConcepts",
                columns: new[] { "SystemUri", "Code" });
        }
    }
}
