using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addforginkeyfordiagonestable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_Doctors_doctorId",
                table: "Diagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_TerminologyCodes_TerminologyCodeId",
                table: "Diagnoses");

            migrationBuilder.RenameColumn(
                name: "doctorId",
                table: "Diagnoses",
                newName: "DoctorId");

            migrationBuilder.RenameColumn(
                name: "TerminologyCodeId",
                table: "Diagnoses",
                newName: "TerminologyCodeLookupId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnoses_doctorId",
                table: "Diagnoses",
                newName: "IX_Diagnoses_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnoses_TerminologyCodeId",
                table: "Diagnoses",
                newName: "IX_Diagnoses_TerminologyCodeLookupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_Doctors_DoctorId",
                table: "Diagnoses",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_TerminologyCodes_TerminologyCodeLookupId",
                table: "Diagnoses",
                column: "TerminologyCodeLookupId",
                principalTable: "TerminologyCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_Doctors_DoctorId",
                table: "Diagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_TerminologyCodes_TerminologyCodeLookupId",
                table: "Diagnoses");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Diagnoses",
                newName: "doctorId");

            migrationBuilder.RenameColumn(
                name: "TerminologyCodeLookupId",
                table: "Diagnoses",
                newName: "TerminologyCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnoses_DoctorId",
                table: "Diagnoses",
                newName: "IX_Diagnoses_doctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnoses_TerminologyCodeLookupId",
                table: "Diagnoses",
                newName: "IX_Diagnoses_TerminologyCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_Doctors_doctorId",
                table: "Diagnoses",
                column: "doctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_TerminologyCodes_TerminologyCodeId",
                table: "Diagnoses",
                column: "TerminologyCodeId",
                principalTable: "TerminologyCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
