using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GamedayTracker.Data.Migrations.BotDb
{
    /// <inheritdoc />
    public partial class EditModel_PoolPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bet_Members_GuildMemberId",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "Picks",
                table: "PlayerPicks");

            migrationBuilder.AlterColumn<string>(
                name: "HomeTeamName",
                table: "Bet",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GuildMemberId",
                table: "Bet",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "AwayTeamName",
                table: "Bet",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PoolPlayerId",
                table: "Bet",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Company = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bet_PoolPlayerId",
                table: "Bet",
                column: "PoolPlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bet_Members_GuildMemberId",
                table: "Bet",
                column: "GuildMemberId",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bet_Players_PoolPlayerId",
                table: "Bet",
                column: "PoolPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bet_Members_GuildMemberId",
                table: "Bet");

            migrationBuilder.DropForeignKey(
                name: "FK_Bet_Players_PoolPlayerId",
                table: "Bet");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Bet_PoolPlayerId",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "PoolPlayerId",
                table: "Bet");

            migrationBuilder.AddColumn<string[]>(
                name: "Picks",
                table: "PlayerPicks",
                type: "text[]",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HomeTeamName",
                table: "Bet",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GuildMemberId",
                table: "Bet",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AwayTeamName",
                table: "Bet",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bet_Members_GuildMemberId",
                table: "Bet",
                column: "GuildMemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
