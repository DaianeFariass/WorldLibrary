using Microsoft.EntityFrameworkCore.Migrations;

namespace WorldLibrary.Web.Migrations
{
    public partial class AddListCustomerToBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_BookId",
                table: "Customers",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Books_BookId",
                table: "Customers",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Books_BookId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_BookId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "Customers");
        }
    }
}
