using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoincPanelComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "LabTestResults");

            migrationBuilder.AddColumn<string>(
                name: "InterpretationFlag",
                table: "LabTestResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NumericValue",
                table: "LabTestResults",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceRange",
                table: "LabTestResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringValue",
                table: "LabTestResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MedicalConceptCatalogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SystemUri = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Component = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Property = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TimeAspect = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    System = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ScaleType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MethodType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Class = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Display = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsRadiology = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalConceptCatalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoincPanelComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentLoincConceptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildLoincConceptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Conditionality = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoincPanelComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoincPanelComponents_MedicalConceptCatalogs_ChildLoincConceptId",
                        column: x => x.ChildLoincConceptId,
                        principalTable: "MedicalConceptCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoincPanelComponents_MedicalConceptCatalogs_ParentLoincConceptId",
                        column: x => x.ParentLoincConceptId,
                        principalTable: "MedicalConceptCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoincPanelComponents_ChildLoincConceptId",
                table: "LoincPanelComponents",
                column: "ChildLoincConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_LoincPanelComponents_ParentLoincConceptId_ChildLoincConceptId",
                table: "LoincPanelComponents",
                columns: new[] { "ParentLoincConceptId", "ChildLoincConceptId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalConceptCatalogs_Class",
                table: "MedicalConceptCatalogs",
                column: "Class");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalConceptCatalogs_Display",
                table: "MedicalConceptCatalogs",
                column: "Display");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalConceptCatalogs_IsRadiology",
                table: "MedicalConceptCatalogs",
                column: "IsRadiology");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalConceptCatalogs_SystemUri_Code",
                table: "MedicalConceptCatalogs",
                columns: new[] { "SystemUri", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoincPanelComponents");

            migrationBuilder.DropTable(
                name: "MedicalConceptCatalogs");

            migrationBuilder.DropColumn(
                name: "InterpretationFlag",
                table: "LabTestResults");

            migrationBuilder.DropColumn(
                name: "NumericValue",
                table: "LabTestResults");

            migrationBuilder.DropColumn(
                name: "ReferenceRange",
                table: "LabTestResults");

            migrationBuilder.DropColumn(
                name: "StringValue",
                table: "LabTestResults");

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "LabTestResults",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
