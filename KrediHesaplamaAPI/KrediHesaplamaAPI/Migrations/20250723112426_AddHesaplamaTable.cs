using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KrediHesaplamaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddHesaplamaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hesaplamalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KrediTipi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Vade = table.Column<int>(type: "int", nullable: false),
                    Faiz = table.Column<double>(type: "float", nullable: false),
                    AylikOdeme = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToplamOdeme = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OdemePlaniJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HesaplamaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hesaplamalar", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hesaplamalar");
        }
    }
}
