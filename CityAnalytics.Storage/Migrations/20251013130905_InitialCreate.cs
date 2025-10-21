using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityAnalytics.Storage.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyInstitutionUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Institution = table.Column<string>(type: "TEXT", nullable: false),
                    FullFare = table.Column<int>(type: "INTEGER", nullable: false),
                    Student = table.Column<int>(type: "INTEGER", nullable: false),
                    Teacher = table.Column<int>(type: "INTEGER", nullable: false),
                    SixtyYearsOld = table.Column<int>(type: "INTEGER", nullable: false),
                    Ticket = table.Column<int>(type: "INTEGER", nullable: false),
                    Child = table.Column<int>(type: "INTEGER", nullable: false),
                    Personnel = table.Column<int>(type: "INTEGER", nullable: false),
                    Free = table.Column<int>(type: "INTEGER", nullable: false),
                    BankCard = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyInstitutionUsages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyInstitutionUsages_Institution_Date",
                table: "DailyInstitutionUsages",
                columns: new[] { "Institution", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyInstitutionUsages");
        }
    }
}
