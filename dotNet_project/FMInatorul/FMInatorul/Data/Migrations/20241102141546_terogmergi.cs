using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMInatorul.Migrations
{
    public partial class terogmergi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FacultateID",
                table: "Professors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Professors_FacultateID",
                table: "Professors",
                column: "FacultateID");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Facultati_FacultateID",
                table: "Professors",
                column: "FacultateID",
                principalTable: "Facultati",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Facultati_FacultateID",
                table: "Professors");

            migrationBuilder.DropIndex(
                name: "IX_Professors_FacultateID",
                table: "Professors");

            migrationBuilder.DropColumn(
                name: "FacultateID",
                table: "Professors");
        }
    }
}
