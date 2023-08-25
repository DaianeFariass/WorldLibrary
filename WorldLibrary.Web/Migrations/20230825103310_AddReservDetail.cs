using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorldLibrary.Web.Migrations
{
    public partial class AddReservDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Book",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Reserves");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryDate",
                table: "Reserves",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "ReserveDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: true),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    ReserveId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReserveDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReserveDetails_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReserveDetails_Reserves_ReserveId",
                        column: x => x.ReserveId,
                        principalTable: "Reserves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReserveDetailsTemp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BookId = table.Column<int>(type: "int", nullable: true),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReserveDetailsTemp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReserveDetailsTemp_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReserveDetailsTemp_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReserveDetails_BookId",
                table: "ReserveDetails",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveDetails_ReserveId",
                table: "ReserveDetails",
                column: "ReserveId");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveDetailsTemp_BookId",
                table: "ReserveDetailsTemp",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveDetailsTemp_UserId",
                table: "ReserveDetailsTemp",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReserveDetails");

            migrationBuilder.DropTable(
                name: "ReserveDetailsTemp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryDate",
                table: "Reserves",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Book",
                table: "Reserves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rate",
                table: "Reserves",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
