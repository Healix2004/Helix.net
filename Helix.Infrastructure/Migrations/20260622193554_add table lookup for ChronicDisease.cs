using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addtablelookupforChronicDisease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChronicDisease_Patients_PatientId",
                table: "ChronicDisease");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChronicDisease",
                table: "ChronicDisease");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AllergenCatalogs");

            migrationBuilder.RenameTable(
                name: "ChronicDisease",
                newName: "ChronicDiseases");

            migrationBuilder.RenameIndex(
                name: "IX_ChronicDisease_PatientId",
                table: "ChronicDiseases",
                newName: "IX_ChronicDiseases_PatientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChronicDiseases",
                table: "ChronicDiseases",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChronicDiseases_Patients_PatientId",
                table: "ChronicDiseases",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChronicDiseases_Patients_PatientId",
                table: "ChronicDiseases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChronicDiseases",
                table: "ChronicDiseases");

            migrationBuilder.RenameTable(
                name: "ChronicDiseases",
                newName: "ChronicDisease");

            migrationBuilder.RenameIndex(
                name: "IX_ChronicDiseases_PatientId",
                table: "ChronicDisease",
                newName: "IX_ChronicDisease_PatientId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "AllergenCatalogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChronicDisease",
                table: "ChronicDisease",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChronicDisease_Patients_PatientId",
                table: "ChronicDisease",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
