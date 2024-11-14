using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMInatorul.Migrations
{
    public partial class AdaugareFacultati : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FacultateID",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FacultateId",
                table: "Materii",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Facultate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nume = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facultate", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_FacultateID",
                table: "Students",
                column: "FacultateID");

            migrationBuilder.CreateIndex(
                name: "IX_Materii_FacultateId",
                table: "Materii",
                column: "FacultateId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materii_Facultate_FacultateId",
                table: "Materii");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Facultate_FacultateID",
                table: "Students");

            migrationBuilder.DropTable(
                name: "Facultate");

            migrationBuilder.DropIndex(
                name: "IX_Students_FacultateID",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Materii_FacultateId",
                table: "Materii");

            migrationBuilder.DropColumn(
                name: "FacultateID",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FacultateId",
                table: "Materii");
        }
    }
}
