using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addforginkeyforsomeoftables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "Observation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Observation_PatientId",
                table: "Observation",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Observation_Patients_PatientId",
                table: "Observation",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Observation_Patients_PatientId",
                table: "Observation");

            migrationBuilder.DropIndex(
                name: "IX_Observation_PatientId",
                table: "Observation");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Observation");
        }
    }
}
