using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class connectlabOrderandRadiologyOrderbyprescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabOrders_Appointments_AppointmentId",
                table: "LabOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyOrders_Appointments_AppointmentId",
                table: "RadiologyOrders");

            migrationBuilder.RenameColumn(
                name: "AppointmentId",
                table: "RadiologyOrders",
                newName: "PrescriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_RadiologyOrders_AppointmentId",
                table: "RadiologyOrders",
                newName: "IX_RadiologyOrders_PrescriptionId");

            migrationBuilder.RenameColumn(
                name: "AppointmentId",
                table: "LabOrders",
                newName: "PrescriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_LabOrders_AppointmentId",
                table: "LabOrders",
                newName: "IX_LabOrders_PrescriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrders_Prescriptions_PrescriptionId",
                table: "LabOrders",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyOrders_Prescriptions_PrescriptionId",
                table: "RadiologyOrders",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabOrders_Prescriptions_PrescriptionId",
                table: "LabOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyOrders_Prescriptions_PrescriptionId",
                table: "RadiologyOrders");

            migrationBuilder.RenameColumn(
                name: "PrescriptionId",
                table: "RadiologyOrders",
                newName: "AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_RadiologyOrders_PrescriptionId",
                table: "RadiologyOrders",
                newName: "IX_RadiologyOrders_AppointmentId");

            migrationBuilder.RenameColumn(
                name: "PrescriptionId",
                table: "LabOrders",
                newName: "AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_LabOrders_PrescriptionId",
                table: "LabOrders",
                newName: "IX_LabOrders_AppointmentId");

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
    }
}
