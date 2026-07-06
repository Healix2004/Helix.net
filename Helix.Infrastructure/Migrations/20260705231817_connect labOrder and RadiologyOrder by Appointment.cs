using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class connectlabOrderandRadiologyOrderbyAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentId",
                table: "RadiologyOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentId",
                table: "LabOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrders_AppointmentId",
                table: "RadiologyOrders",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrders_AppointmentId",
                table: "LabOrders",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrders_Appointments_AppointmentId",
                table: "LabOrders",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyOrders_Appointments_AppointmentId",
                table: "RadiologyOrders",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabOrders_Appointments_AppointmentId",
                table: "LabOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyOrders_Appointments_AppointmentId",
                table: "RadiologyOrders");

            migrationBuilder.DropIndex(
                name: "IX_RadiologyOrders_AppointmentId",
                table: "RadiologyOrders");

            migrationBuilder.DropIndex(
                name: "IX_LabOrders_AppointmentId",
                table: "LabOrders");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "RadiologyOrders");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "LabOrders");
        }
    }
}
