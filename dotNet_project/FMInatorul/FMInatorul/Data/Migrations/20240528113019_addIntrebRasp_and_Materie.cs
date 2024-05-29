using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMInatorul.Migrations
{
    public partial class addIntrebRasp_and_Materie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntrebariRasps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    intrebare = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    raspunsCorect = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    validareProfesor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntrebariRasps", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Variantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Choice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IntrebariRaspId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Variantes_IntrebariRasps_IntrebariRaspId",
                        column: x => x.IntrebariRaspId,
                        principalTable: "IntrebariRasps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterieIntRas_Materie_id",
                table: "MaterieIntRas",
                column: "Materie_id");

            migrationBuilder.CreateIndex(
                name: "IX_Variantes_IntrebariRaspId",
                table: "Variantes",
                column: "IntrebariRaspId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterieIntRas");

            migrationBuilder.DropTable(
                name: "Variantes");

            migrationBuilder.DropTable(
                name: "IntrebariRasps");
        }
    }
}
