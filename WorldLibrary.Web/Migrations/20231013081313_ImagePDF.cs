using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorldLibrary.Web.Migrations
{
    public partial class ImagePDF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookPdfUrl",
                table: "Books");

            migrationBuilder.AddColumn<Guid>(
                name: "ImagePdf",
                table: "Books",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePdf",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "BookPdfUrl",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
