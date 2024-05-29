using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMInatorul.Migrations
{
    public partial class change_materie_intrebariRasp_one_to_many : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterieIntRas");

            migrationBuilder.AddColumn<int>(
                name: "MaterieId",
                table: "IntrebariRasps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IntrebariRasps_MaterieId",
                table: "IntrebariRasps",
                column: "MaterieId");

            migrationBuilder.AddForeignKey(
                name: "FK_IntrebariRasps_Materii_MaterieId",
                table: "IntrebariRasps",
                column: "MaterieId",
                principalTable: "Materii",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntrebariRasps_Materii_MaterieId",
                table: "IntrebariRasps");

            migrationBuilder.DropIndex(
                name: "IX_IntrebariRasps_MaterieId",
                table: "IntrebariRasps");

            migrationBuilder.DropColumn(
                name: "MaterieId",
                table: "IntrebariRasps");

            migrationBuilder.CreateTable(
                name: "MaterieIntRas",
                columns: table => new
                {
                    IntRas_id = table.Column<int>(type: "int", nullable: false),
                    Materie_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterieIntRas", x => new { x.IntRas_id, x.Materie_id });
                    table.ForeignKey(
                        name: "FK_MaterieIntRas_IntrebariRasps_IntRas_id",
                        column: x => x.IntRas_id,
                        principalTable: "IntrebariRasps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterieIntRas_Materii_Materie_id",
                        column: x => x.Materie_id,
                        principalTable: "Materii",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterieIntRas_Materie_id",
                table: "MaterieIntRas",
                column: "Materie_id");
        }
    }
}
