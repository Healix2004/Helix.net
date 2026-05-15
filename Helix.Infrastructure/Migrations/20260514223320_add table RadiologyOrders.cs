using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addtableRadiologyOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabOrders_Doctors_DoctorId",
                table: "LabOrders");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "RadiologyImage",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "RadiologyImage",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "RadiologyOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TerminologyCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QrToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyOrders_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyOrders_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyOrders_TerminologyCodes_TerminologyCodeId",
                        column: x => x.TerminologyCodeId,
                        principalTable: "TerminologyCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyResults_OrderId",
                table: "RadiologyResults",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrders_DoctorId",
                table: "RadiologyOrders",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrders_PatientId",
                table: "RadiologyOrders",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrders_TerminologyCodeId",
                table: "RadiologyOrders",
                column: "TerminologyCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrders_Doctors_DoctorId",
                table: "LabOrders",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyResults_RadiologyOrders_OrderId",
                table: "RadiologyResults",
                column: "OrderId",
                principalTable: "RadiologyOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabOrders_Doctors_DoctorId",
                table: "LabOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyResults_RadiologyOrders_OrderId",
                table: "RadiologyResults");

            migrationBuilder.DropTable(
                name: "RadiologyOrders");

            migrationBuilder.DropIndex(
                name: "IX_RadiologyResults_OrderId",
                table: "RadiologyResults");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "RadiologyImage",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "RadiologyImage",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrders_Doctors_DoctorId",
                table: "LabOrders",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
