using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamedayTracker.Data.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class UpdateModel_TeamStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TeamName",
                table: "TeamStats",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LineType",
                table: "TeamStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineType",
                table: "TeamStats");

            migrationBuilder.AlterColumn<string>(
                name: "TeamName",
                table: "TeamStats",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
