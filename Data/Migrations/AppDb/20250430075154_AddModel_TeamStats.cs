using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GamedayTracker.Data.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class AddModel_TeamStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "TeamStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    TeamName = table.Column<string>(type: "text", nullable: true),
                    GamesPlayed = table.Column<int>(type: "integer", nullable: false),
                    TotalPoints = table.Column<int>(type: "integer", nullable: false),
                    PointsPerGame = table.Column<double>(type: "double precision", nullable: false),
                    RushYardsTotal = table.Column<int>(type: "integer", nullable: false),
                    RushPerGame = table.Column<double>(type: "double precision", nullable: false),
                    PassYardsTotal = table.Column<int>(type: "integer", nullable: false),
                    PassYardsPerGame = table.Column<double>(type: "double precision", nullable: false),
                    TotalYards = table.Column<int>(type: "integer", nullable: false),
                    YardsPerGame = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamStats", x => x.Id);
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamStats");
        }
    }
}
