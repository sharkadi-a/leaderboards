using Microsoft.EntityFrameworkCore.Migrations;

namespace AndreyGames.Leaderboards.Service.Migrations
{
    public partial class AddCaseInsensitiveIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE INDEX IX_Entries_LeaderboardId_LowerPlayerName ON \"Entries\" (\"LeaderboardId\", LOWER(\"PlayerName\"));");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Entries_LeaderboardId_LowerPlayerName",
                table: "Entries");
        }
    }
}
