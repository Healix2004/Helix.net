using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removedoctorforginkeyfromtablelabtestOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestResults_Doctors_DoctorId",
                table: "LabTestResults");

            migrationBuilder.DropIndex(
                name: "IX_LabTestResults_DoctorId",
                table: "LabTestResults");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "LabTestResults");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "LabTestResults",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LabTestResults_DoctorId",
                table: "LabTestResults",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestResults_Doctors_DoctorId",
                table: "LabTestResults",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
