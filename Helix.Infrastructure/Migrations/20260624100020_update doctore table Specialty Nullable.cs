using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedoctoretableSpecialtyNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Specialty",
                table: "Doctors");

            migrationBuilder.AddColumn<string>(
                name: "SpecialtyCatalogCode",
                table: "Doctors",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_SpecialtyCatalogCode",
                table: "Doctors",
                column: "SpecialtyCatalogCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_SpecialtyCatalogs_SpecialtyCatalogCode",
                table: "Doctors",
                column: "SpecialtyCatalogCode",
                principalTable: "SpecialtyCatalogs",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_SpecialtyCatalogs_SpecialtyCatalogCode",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_SpecialtyCatalogCode",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "SpecialtyCatalogCode",
                table: "Doctors");

            migrationBuilder.AddColumn<string>(
                name: "Specialty",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
