using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMInatorul.Migrations
{
    public partial class ProfesorDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profesors_AspNetUsers_ApplicationUserId",
                table: "Profesors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profesors",
                table: "Profesors");

            migrationBuilder.RenameTable(
                name: "Profesors",
                newName: "Professors");

            migrationBuilder.RenameIndex(
                name: "IX_Profesors_ApplicationUserId",
                table: "Professors",
                newName: "IX_Professors_ApplicationUserId");

            migrationBuilder.AddColumn<int>(
                name: "MaterieId",
                table: "Professors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Professors",
                table: "Professors",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Professors_MaterieId",
                table: "Professors",
                column: "MaterieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_AspNetUsers_ApplicationUserId",
                table: "Professors",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Materii_MaterieId",
                table: "Professors",
                column: "MaterieId",
                principalTable: "Materii",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_AspNetUsers_ApplicationUserId",
                table: "Professors");

            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Materii_MaterieId",
                table: "Professors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Professors",
                table: "Professors");

            migrationBuilder.DropIndex(
                name: "IX_Professors_MaterieId",
                table: "Professors");

            migrationBuilder.DropColumn(
                name: "MaterieId",
                table: "Professors");

            migrationBuilder.RenameTable(
                name: "Professors",
                newName: "Profesors");

            migrationBuilder.RenameIndex(
                name: "IX_Professors_ApplicationUserId",
                table: "Profesors",
                newName: "IX_Profesors_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profesors",
                table: "Profesors",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Profesors_AspNetUsers_ApplicationUserId",
                table: "Profesors",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
