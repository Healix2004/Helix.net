using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatechronicDisease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ChronicDiseases");

            migrationBuilder.AddColumn<string>(
                name: "ChronicDiseaseCatalogCode",
                table: "ChronicDiseases",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ChronicDiseases_ChronicDiseaseCatalogCode",
                table: "ChronicDiseases",
                column: "ChronicDiseaseCatalogCode");

            migrationBuilder.AddForeignKey(
                name: "FK_ChronicDiseases_ChronicDiseaseCatalogs_ChronicDiseaseCatalogCode",
                table: "ChronicDiseases",
                column: "ChronicDiseaseCatalogCode",
                principalTable: "ChronicDiseaseCatalogs",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChronicDiseases_ChronicDiseaseCatalogs_ChronicDiseaseCatalogCode",
                table: "ChronicDiseases");

            migrationBuilder.DropIndex(
                name: "IX_ChronicDiseases_ChronicDiseaseCatalogCode",
                table: "ChronicDiseases");

            migrationBuilder.DropColumn(
                name: "ChronicDiseaseCatalogCode",
                table: "ChronicDiseases");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ChronicDiseases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
