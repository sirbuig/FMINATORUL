using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMInatorul.Migrations
{
    public partial class AdaugareFacultati2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materii_Facultate_FacultateId",
                table: "Materii");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Facultate_FacultateID",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Facultate",
                table: "Facultate");

            migrationBuilder.RenameTable(
                name: "Facultate",
                newName: "Facultati");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Facultati",
                table: "Facultati",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Materii_Facultati_FacultateId",
                table: "Materii",
                column: "FacultateId",
                principalTable: "Facultati",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Facultati_FacultateID",
                table: "Students",
                column: "FacultateID",
                principalTable: "Facultati",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materii_Facultati_FacultateId",
                table: "Materii");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Facultati_FacultateID",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Facultati",
                table: "Facultati");

            migrationBuilder.RenameTable(
                name: "Facultati",
                newName: "Facultate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Facultate",
                table: "Facultate",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Materii_Facultate_FacultateId",
                table: "Materii",
                column: "FacultateId",
                principalTable: "Facultate",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Facultate_FacultateID",
                table: "Students",
                column: "FacultateID",
                principalTable: "Facultate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
