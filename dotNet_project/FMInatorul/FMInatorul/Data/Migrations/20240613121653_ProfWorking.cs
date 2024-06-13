using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMInatorul.Migrations
{
    public partial class ProfWorking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Materii_MaterieId",
                table: "Professors");

            migrationBuilder.AlterColumn<int>(
                name: "MaterieId",
                table: "Professors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Materii_MaterieId",
                table: "Professors",
                column: "MaterieId",
                principalTable: "Materii",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Materii_MaterieId",
                table: "Professors");

            migrationBuilder.AlterColumn<int>(
                name: "MaterieId",
                table: "Professors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Materii_MaterieId",
                table: "Professors",
                column: "MaterieId",
                principalTable: "Materii",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
