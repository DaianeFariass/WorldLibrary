using Microsoft.EntityFrameworkCore.Migrations;

namespace WorldLibrary.Web.Migrations
{
    public partial class AddPhysicalLibraryToReserve : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhysicalLibraryId",
                table: "Reserves",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhysicalLibraryId",
                table: "ReserveDetailsTemp",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhysicalLibraryId",
                table: "ReserveDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reserves_PhysicalLibraryId",
                table: "Reserves",
                column: "PhysicalLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveDetailsTemp_PhysicalLibraryId",
                table: "ReserveDetailsTemp",
                column: "PhysicalLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveDetails_PhysicalLibraryId",
                table: "ReserveDetails",
                column: "PhysicalLibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReserveDetails_PhysicalLibraries_PhysicalLibraryId",
                table: "ReserveDetails",
                column: "PhysicalLibraryId",
                principalTable: "PhysicalLibraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReserveDetailsTemp_PhysicalLibraries_PhysicalLibraryId",
                table: "ReserveDetailsTemp",
                column: "PhysicalLibraryId",
                principalTable: "PhysicalLibraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_PhysicalLibraries_PhysicalLibraryId",
                table: "Reserves",
                column: "PhysicalLibraryId",
                principalTable: "PhysicalLibraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReserveDetails_PhysicalLibraries_PhysicalLibraryId",
                table: "ReserveDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReserveDetailsTemp_PhysicalLibraries_PhysicalLibraryId",
                table: "ReserveDetailsTemp");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserves_PhysicalLibraries_PhysicalLibraryId",
                table: "Reserves");

            migrationBuilder.DropIndex(
                name: "IX_Reserves_PhysicalLibraryId",
                table: "Reserves");

            migrationBuilder.DropIndex(
                name: "IX_ReserveDetailsTemp_PhysicalLibraryId",
                table: "ReserveDetailsTemp");

            migrationBuilder.DropIndex(
                name: "IX_ReserveDetails_PhysicalLibraryId",
                table: "ReserveDetails");

            migrationBuilder.DropColumn(
                name: "PhysicalLibraryId",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "PhysicalLibraryId",
                table: "ReserveDetailsTemp");

            migrationBuilder.DropColumn(
                name: "PhysicalLibraryId",
                table: "ReserveDetails");
        }
    }
}
