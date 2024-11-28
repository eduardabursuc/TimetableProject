using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_timeslots",
                table: "timeslots");

            migrationBuilder.DropIndex(
                name: "IX_timeslots_TimetableId",
                table: "timeslots");

            migrationBuilder.AddPrimaryKey(
                name: "PK_timeslots",
                table: "timeslots",
                columns: new[] { "TimetableId", "Time", "Day", "RoomName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_timeslots",
                table: "timeslots");

            migrationBuilder.AddPrimaryKey(
                name: "PK_timeslots",
                table: "timeslots",
                columns: new[] { "Time", "Day", "RoomName" });

            migrationBuilder.CreateIndex(
                name: "IX_timeslots_TimetableId",
                table: "timeslots",
                column: "TimetableId");
        }
    }
}
