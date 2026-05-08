using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addConsentstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Consents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsentGrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsentExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEmergencyOverride = table.Column<bool>(type: "bit", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consents_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consents_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consents_DoctorId",
                table: "Consents",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Consents_PatientId",
                table: "Consents",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consents");
        }
    }
}
