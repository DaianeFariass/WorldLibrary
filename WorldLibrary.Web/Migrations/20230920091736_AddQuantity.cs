using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorldLibrary.Web.Migrations
{
    public partial class AddQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReserveDetails_Reserves_ReserveId",
                table: "ReserveDetails");

            migrationBuilder.DropColumn(
                name: "BookingDate",
                table: "ReserveDetailsTemp");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "ReserveDetailsTemp");

            migrationBuilder.DropColumn(
                name: "BookingDate",
                table: "ReserveDetails");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "ReserveDetails");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "ReserveId",
                table: "ReserveDetails",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_ReserveDetails_ReserveId",
                table: "ReserveDetails",
                newName: "IX_ReserveDetails_CustomerId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualReturnDate",
                table: "Reserves",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "Reserves",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Reserves",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "Reserves",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "Reserves",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "Reserves",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusReserve",
                table: "Reserves",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ReserveDetailsTemp",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "Books",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "StatusBook",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reserves_BookId",
                table: "Reserves",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Reserves_CustomerId",
                table: "Reserves",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveDetailsTemp_CustomerId",
                table: "ReserveDetailsTemp",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReserveDetails_Customers_CustomerId",
                table: "ReserveDetails",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReserveDetailsTemp_Customers_CustomerId",
                table: "ReserveDetailsTemp",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_Books_BookId",
                table: "Reserves",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_Customers_CustomerId",
                table: "Reserves",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReserveDetails_Customers_CustomerId",
                table: "ReserveDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReserveDetailsTemp_Customers_CustomerId",
                table: "ReserveDetailsTemp");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserves_Books_BookId",
                table: "Reserves");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserves_Customers_CustomerId",
                table: "Reserves");

            migrationBuilder.DropIndex(
                name: "IX_Reserves_BookId",
                table: "Reserves");

            migrationBuilder.DropIndex(
                name: "IX_Reserves_CustomerId",
                table: "Reserves");

            migrationBuilder.DropIndex(
                name: "IX_ReserveDetailsTemp_CustomerId",
                table: "ReserveDetailsTemp");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Name",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "ActualReturnDate",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "StatusReserve",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ReserveDetailsTemp");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "StatusBook",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "ReserveDetails",
                newName: "ReserveId");

            migrationBuilder.RenameIndex(
                name: "IX_ReserveDetails_CustomerId",
                table: "ReserveDetails",
                newName: "IX_ReserveDetails_ReserveId");

            migrationBuilder.AddColumn<DateTime>(
                name: "BookingDate",
                table: "ReserveDetailsTemp",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "ReserveDetailsTemp",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "BookingDate",
                table: "ReserveDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "ReserveDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ReserveDetails_Reserves_ReserveId",
                table: "ReserveDetails",
                column: "ReserveId",
                principalTable: "Reserves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
