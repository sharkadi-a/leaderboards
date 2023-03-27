using Microsoft.EntityFrameworkCore.Migrations;

namespace AndreyGames.Leaderboards.Service.Migrations
{
    public partial class AddMoreIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Entries_LeaderboardId_IsWinner_Timestamp_PlayerName",
                table: "Entries",
                columns: new[] { "LeaderboardId", "IsWinner", "Timestamp", "PlayerName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Entries_LeaderboardId_IsWinner_Timestamp_PlayerName",
                table: "Entries");
        }
    }
}
