using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addradiologytables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RadiologyResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Impression = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyResults_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RadiologyResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileSizeInKB = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyImage_RadiologyResults_RadiologyResultId",
                        column: x => x.RadiologyResultId,
                        principalTable: "RadiologyResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyImage_RadiologyResultId",
                table: "RadiologyImage",
                column: "RadiologyResultId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyResults_PatientId",
                table: "RadiologyResults",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RadiologyImage");

            migrationBuilder.DropTable(
                name: "RadiologyResults");
        }
    }
}
