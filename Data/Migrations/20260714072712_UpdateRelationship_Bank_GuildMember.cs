using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GamedayTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationship_Bank_GuildMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bet_Members_GuildMemberId",
                table: "Bet");

            migrationBuilder.DropForeignKey(
                name: "FK_Bet_Players_PoolPlayerId",
                table: "Bet");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Bank_BankId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_PlayerPicks_PlayerPicksId",
                table: "Members");

            migrationBuilder.DropTable(
                name: "PlayerPicks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_BankId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_PlayerPicksId",
                table: "Members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds");

            migrationBuilder.DropIndex(
                name: "IX_Bet_GuildMemberId",
                table: "Bet");

            migrationBuilder.DropIndex(
                name: "IX_Bet_PoolPlayerId",
                table: "Bet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bank",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "PlayerPicksId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "AwayTeamName",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "BetAmount",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "GuildMemberId",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "HomeTeamName",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "DepositAmount",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "LastDeposit",
                table: "Bank");

            migrationBuilder.RenameColumn(
                name: "PoolWins",
                table: "Members",
                newName: "BetWins");

            migrationBuilder.RenameColumn(
                name: "PoolPlayerId",
                table: "Bet",
                newName: "Type");

            migrationBuilder.AlterColumn<string>(
                name: "PlayerName",
                table: "Players",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Company",
                table: "Players",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "Players",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DepositTimestamp",
                table: "Players",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "GuildMemberId",
                table: "Players",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GuildMemberMemberId",
                table: "Players",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "Picks",
                table: "Players",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PlayerId",
                table: "Players",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "MemberId",
                table: "Members",
                type: "numeric(20,0)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<decimal>(
                name: "GuildId",
                table: "Members",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "GuildName",
                table: "Members",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "NotificationChannelId",
                table: "Guilds",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "GuildOwnerId",
                table: "Guilds",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "GuildId",
                table: "Guilds",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<bool>(
                name: "IsDailyHeadlinesEnabled",
                table: "Guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRealTimeScoresEnabled",
                table: "Guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReceiveSystemMessages",
                table: "Guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Bet",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "EventId",
                table: "Bet",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "GameDate",
                table: "Bet",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<decimal>(
                name: "GuildMemberMemberId",
                table: "Bet",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Multiplier",
                table: "Bet",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Odds",
                table: "Bet",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Payout",
                table: "Bet",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlacedAt",
                table: "Bet",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Selection",
                table: "Bet",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Bet",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UserId",
                table: "Bet",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WagerAmount",
                table: "Bet",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Bank",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<decimal>(
                name: "BankId",
                table: "Bank",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GuildMemberId",
                table: "Bank",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LastDepositAmount",
                table: "Bank",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                column: "MemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds",
                column: "GuildId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bank",
                table: "Bank",
                column: "BankId");

            migrationBuilder.CreateTable(
                name: "DailyNumberPick",
                columns: table => new
                {
                    GuildMemberMemberId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Numbers = table.Column<int[]>(type: "integer[]", nullable: false),
                    PlayType = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyNumberPick", x => new { x.GuildMemberMemberId, x.Id });
                    table.ForeignKey(
                        name: "FK_DailyNumberPick_Members_GuildMemberMemberId",
                        column: x => x.GuildMemberMemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    TicketId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ThreadId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ClosedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    Details = table.Column<string>(type: "text", nullable: true),
                    GuildMemberMemberId = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_Ticket_Members_GuildMemberMemberId",
                        column: x => x.GuildMemberMemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_GuildMemberMemberId",
                table: "Players",
                column: "GuildMemberMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Bet_GuildMemberMemberId",
                table: "Bet",
                column: "GuildMemberMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Bank_GuildMemberId",
                table: "Bank",
                column: "GuildMemberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_GuildMemberMemberId",
                table: "Ticket",
                column: "GuildMemberMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bank_Members_GuildMemberId",
                table: "Bank",
                column: "GuildMemberId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bet_Members_GuildMemberMemberId",
                table: "Bet",
                column: "GuildMemberMemberId",
                principalTable: "Members",
                principalColumn: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Members_GuildMemberMemberId",
                table: "Players",
                column: "GuildMemberMemberId",
                principalTable: "Members",
                principalColumn: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bank_Members_GuildMemberId",
                table: "Bank");

            migrationBuilder.DropForeignKey(
                name: "FK_Bet_Members_GuildMemberMemberId",
                table: "Bet");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Members_GuildMemberMemberId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "DailyNumberPick");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Players_GuildMemberMemberId",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Members",
                table: "Members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds");

            migrationBuilder.DropIndex(
                name: "IX_Bet_GuildMemberMemberId",
                table: "Bet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bank",
                table: "Bank");

            migrationBuilder.DropIndex(
                name: "IX_Bank_GuildMemberId",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "DepositTimestamp",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GuildMemberId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GuildMemberMemberId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Picks",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GuildName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "IsDailyHeadlinesEnabled",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "IsRealTimeScoresEnabled",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "ReceiveSystemMessages",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "GameDate",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "GuildMemberMemberId",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "Multiplier",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "Odds",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "Payout",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "PlacedAt",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "Selection",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "WagerAmount",
                table: "Bet");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "GuildMemberId",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "LastDepositAmount",
                table: "Bank");

            migrationBuilder.RenameColumn(
                name: "BetWins",
                table: "Members",
                newName: "PoolWins");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Bet",
                newName: "PoolPlayerId");

            migrationBuilder.AlterColumn<string>(
                name: "PlayerName",
                table: "Players",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Company",
                table: "Players",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "GuildId",
                table: "Members",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<string>(
                name: "MemberId",
                table: "Members",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Members",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "Members",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerPicksId",
                table: "Members",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "NotificationChannelId",
                table: "Guilds",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "GuildOwnerId",
                table: "Guilds",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<long>(
                name: "GuildId",
                table: "Guilds",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Bet",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "AwayTeamName",
                table: "Bet",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BetAmount",
                table: "Bet",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "GuildMemberId",
                table: "Bet",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeTeamName",
                table: "Bet",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Balance",
                table: "Bank",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Bank",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<double>(
                name: "DepositAmount",
                table: "Bank",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LastDeposit",
                table: "Bank",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Members",
                table: "Members",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bank",
                table: "Bank",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PlayerPicks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Week = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPicks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_BankId",
                table: "Members",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_PlayerPicksId",
                table: "Members",
                column: "PlayerPicksId");

            migrationBuilder.CreateIndex(
                name: "IX_Bet_GuildMemberId",
                table: "Bet",
                column: "GuildMemberId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Bank_BankId",
                table: "Members",
                column: "BankId",
                principalTable: "Bank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_PlayerPicks_PlayerPicksId",
                table: "Members",
                column: "PlayerPicksId",
                principalTable: "PlayerPicks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
