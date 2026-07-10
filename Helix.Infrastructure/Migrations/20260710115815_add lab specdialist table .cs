using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addlabspecdialisttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacies_AspNetUsers_AppUserId1",
                table: "Pharmacies");

            migrationBuilder.DropIndex(
                name: "IX_Pharmacies_AppUserId1",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "AppUserId1",
                table: "Pharmacies");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Pharmacies",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "LabSpecialists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LabName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabSpecialistLicenseUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalIdUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabSpecialists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabSpecialists_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacies_AppUserId",
                table: "Pharmacies",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LabSpecialists_AppUserId",
                table: "LabSpecialists",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacies_AspNetUsers_AppUserId",
                table: "Pharmacies",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacies_AspNetUsers_AppUserId",
                table: "Pharmacies");

            migrationBuilder.DropTable(
                name: "LabSpecialists");

            migrationBuilder.DropIndex(
                name: "IX_Pharmacies_AppUserId",
                table: "Pharmacies");

            migrationBuilder.AlterColumn<Guid>(
                name: "AppUserId",
                table: "Pharmacies",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId1",
                table: "Pharmacies",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacies_AppUserId1",
                table: "Pharmacies",
                column: "AppUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacies_AspNetUsers_AppUserId1",
                table: "Pharmacies",
                column: "AppUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
