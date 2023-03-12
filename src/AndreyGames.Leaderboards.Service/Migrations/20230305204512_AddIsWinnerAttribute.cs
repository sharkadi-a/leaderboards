using Microsoft.EntityFrameworkCore.Migrations;

namespace AndreyGames.Leaderboards.Service.Migrations
{
    public partial class AddIsWinnerAttribute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Entries_LeaderboardId_PlayerName",
                table: "Entries");

            migrationBuilder.AddColumn<bool>(
                name: "IsWinner",
                table: "Entries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Entries_LeaderboardId_IsWinner_PlayerName",
                table: "Entries",
                columns: new[] { "LeaderboardId", "IsWinner", "PlayerName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Entries_LeaderboardId_IsWinner_PlayerName",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "IsWinner",
                table: "Entries");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_LeaderboardId_PlayerName",
                table: "Entries",
                columns: new[] { "LeaderboardId", "PlayerName" },
                unique: true);
        }
    }
}
