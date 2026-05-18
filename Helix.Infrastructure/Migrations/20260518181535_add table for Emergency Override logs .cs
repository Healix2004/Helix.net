using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addtableforEmergencyOverridelogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmergencyOverride",
                table: "Consents");

            migrationBuilder.CreateTable(
                name: "emergencyOverrideLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JustificationReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OverrideTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsReviewedByAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emergencyOverrideLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_emergencyOverrideLogs_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_emergencyOverrideLogs_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_emergencyOverrideLogs_DoctorId",
                table: "emergencyOverrideLogs",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_emergencyOverrideLogs_PatientId",
                table: "emergencyOverrideLogs",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "emergencyOverrideLogs");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmergencyOverride",
                table: "Consents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
