using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMInatorul.Migrations
{
    public partial class ProfMaterii : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Materii_MaterieId",
                table: "Professors");

            migrationBuilder.DropIndex(
                name: "IX_Professors_MaterieId",
                table: "Professors");

            migrationBuilder.DropColumn(
                name: "MaterieId",
                table: "Professors");

            migrationBuilder.AddColumn<int>(
                name: "ProfesorId",
                table: "Materii",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materii_ProfesorId",
                table: "Materii",
                column: "ProfesorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materii_Professors_ProfesorId",
                table: "Materii",
                column: "ProfesorId",
                principalTable: "Professors",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materii_Professors_ProfesorId",
                table: "Materii");

            migrationBuilder.DropIndex(
                name: "IX_Materii_ProfesorId",
                table: "Materii");

            migrationBuilder.DropColumn(
                name: "ProfesorId",
                table: "Materii");

            migrationBuilder.AddColumn<int>(
                name: "MaterieId",
                table: "Professors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professors_MaterieId",
                table: "Professors",
                column: "MaterieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Materii_MaterieId",
                table: "Professors",
                column: "MaterieId",
                principalTable: "Materii",
                principalColumn: "Id");
        }
    }
}
