using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMInatorul.Migrations
{
    public partial class MaterieFaculta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materii_Facultati_FacultateId",
                table: "Materii");

            migrationBuilder.RenameColumn(
                name: "FacultateId",
                table: "Materii",
                newName: "FacultateID");

            migrationBuilder.RenameIndex(
                name: "IX_Materii_FacultateId",
                table: "Materii",
                newName: "IX_Materii_FacultateID");

            migrationBuilder.AlterColumn<int>(
                name: "FacultateID",
                table: "Materii",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Materii_Facultati_FacultateID",
                table: "Materii",
                column: "FacultateID",
                principalTable: "Facultati",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materii_Facultati_FacultateID",
                table: "Materii");

            migrationBuilder.RenameColumn(
                name: "FacultateID",
                table: "Materii",
                newName: "FacultateId");

            migrationBuilder.RenameIndex(
                name: "IX_Materii_FacultateID",
                table: "Materii",
                newName: "IX_Materii_FacultateId");

            migrationBuilder.AlterColumn<int>(
                name: "FacultateId",
                table: "Materii",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Materii_Facultati_FacultateId",
                table: "Materii",
                column: "FacultateId",
                principalTable: "Facultati",
                principalColumn: "Id");
        }
    }
}
