using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "timetables",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "timetables",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "timetables",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Event_TimeInterval",
                table: "timeslots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "rooms",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "professors",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "groups",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "courses",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AccountType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Email);
                });

            migrationBuilder.CreateIndex(
                name: "IX_timetables_UserId",
                table: "timetables",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_UserId",
                table: "rooms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_professors_UserId",
                table: "professors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_groups_UserId",
                table: "groups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_courses_UserId",
                table: "courses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_courses_users_UserId",
                table: "courses",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_groups_users_UserId",
                table: "groups",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_professors_users_UserId",
                table: "professors",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_users_UserId",
                table: "rooms",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_timetables_users_UserId",
                table: "timetables",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_users_UserId",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "FK_groups_users_UserId",
                table: "groups");

            migrationBuilder.DropForeignKey(
                name: "FK_professors_users_UserId",
                table: "professors");

            migrationBuilder.DropForeignKey(
                name: "FK_rooms_users_UserId",
                table: "rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_timetables_users_UserId",
                table: "timetables");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropIndex(
                name: "IX_timetables_UserId",
                table: "timetables");

            migrationBuilder.DropIndex(
                name: "IX_rooms_UserId",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "IX_professors_UserId",
                table: "professors");

            migrationBuilder.DropIndex(
                name: "IX_groups_UserId",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "IX_courses_UserId",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "timetables");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "timetables");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "timetables");

            migrationBuilder.DropColumn(
                name: "Event_TimeInterval",
                table: "timeslots");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "professors");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "courses");
        }
    }
}
