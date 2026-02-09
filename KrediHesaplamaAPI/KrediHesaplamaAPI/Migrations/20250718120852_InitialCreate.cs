using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KrediHesaplamaAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KrediUrunleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FaizOrani = table.Column<double>(type: "float", nullable: false),
                    MinTutar = table.Column<double>(type: "float", nullable: false),
                    MaxTutar = table.Column<double>(type: "float", nullable: false),
                    MinVade = table.Column<int>(type: "int", nullable: false),
                    MaxVade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KrediUrunleri", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KrediUrunleri");
        }
    }
}
