using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMInatorul.Data.Migrations
{
    public partial class DoamneAjuta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdApplicationUser",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IdApplicationUser",
                table: "Profesors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdApplicationUser",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdApplicationUser",
                table: "Profesors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
