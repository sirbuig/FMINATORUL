using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMInatorul.Data.Migrations
{
    public partial class ProfStud : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdProf",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdStud",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdProf",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdStud",
                table: "AspNetUsers");
        }
    }
}
