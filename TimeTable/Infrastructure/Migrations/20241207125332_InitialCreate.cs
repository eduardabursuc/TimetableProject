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
            migrationBuilder.DropForeignKey(
                name: "FK_Course_UserEmail",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "FK_courses_users_UserEmail1",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Group_UserEmail",
                table: "groups");

            migrationBuilder.DropForeignKey(
                name: "FK_groups_users_UserEmail1",
                table: "groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Professor_UserEmail",
                table: "professors");

            migrationBuilder.DropForeignKey(
                name: "FK_professors_users_UserEmail1",
                table: "professors");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_UserEmail",
                table: "rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_rooms_users_UserEmail1",
                table: "rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Timetable_UserEmail",
                table: "timetables");

            migrationBuilder.DropForeignKey(
                name: "FK_timetables_users_UserEmail1",
                table: "timetables");

            migrationBuilder.DropIndex(
                name: "IX_timetables_UserEmail1",
                table: "timetables");

            migrationBuilder.DropIndex(
                name: "IX_rooms_UserEmail1",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "IX_professors_UserEmail1",
                table: "professors");

            migrationBuilder.DropIndex(
                name: "IX_groups_UserEmail1",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "IX_courses_UserEmail1",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "UserEmail1",
                table: "timetables");

            migrationBuilder.DropColumn(
                name: "UserEmail1",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "UserEmail1",
                table: "professors");

            migrationBuilder.DropColumn(
                name: "UserEmail1",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "UserEmail1",
                table: "courses");

            migrationBuilder.AddForeignKey(
                name: "FK_courses_users_UserEmail",
                table: "courses",
                column: "UserEmail",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_groups_users_UserEmail",
                table: "groups",
                column: "UserEmail",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_professors_users_UserEmail",
                table: "professors",
                column: "UserEmail",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_users_UserEmail",
                table: "rooms",
                column: "UserEmail",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_timetables_users_UserEmail",
                table: "timetables",
                column: "UserEmail",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_users_UserEmail",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "FK_groups_users_UserEmail",
                table: "groups");

            migrationBuilder.DropForeignKey(
                name: "FK_professors_users_UserEmail",
                table: "professors");

            migrationBuilder.DropForeignKey(
                name: "FK_rooms_users_UserEmail",
                table: "rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_timetables_users_UserEmail",
                table: "timetables");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail1",
                table: "timetables",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserEmail1",
                table: "rooms",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserEmail1",
                table: "professors",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserEmail1",
                table: "groups",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserEmail1",
                table: "courses",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_timetables_UserEmail1",
                table: "timetables",
                column: "UserEmail1");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_UserEmail1",
                table: "rooms",
                column: "UserEmail1");

            migrationBuilder.CreateIndex(
                name: "IX_professors_UserEmail1",
                table: "professors",
                column: "UserEmail1");

            migrationBuilder.CreateIndex(
                name: "IX_groups_UserEmail1",
                table: "groups",
                column: "UserEmail1");

            migrationBuilder.CreateIndex(
                name: "IX_courses_UserEmail1",
                table: "courses",
                column: "UserEmail1");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_UserEmail",
                table: "courses",
                column: "UserEmail",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_courses_users_UserEmail1",
                table: "courses",
                column: "UserEmail1",
                principalTable: "users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Group_UserEmail",
                table: "groups",
                column: "UserEmail",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_groups_users_UserEmail1",
                table: "groups",
                column: "UserEmail1",
                principalTable: "users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Professor_UserEmail",
                table: "professors",
                column: "UserEmail",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_professors_users_UserEmail1",
                table: "professors",
                column: "UserEmail1",
                principalTable: "users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_UserEmail",
                table: "rooms",
                column: "UserEmail",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_users_UserEmail1",
                table: "rooms",
                column: "UserEmail1",
                principalTable: "users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Timetable_UserEmail",
                table: "timetables",
                column: "UserEmail",
                principalTable: "users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_timetables_users_UserEmail1",
                table: "timetables",
                column: "UserEmail1",
                principalTable: "users",
                principalColumn: "Email");
        }
    }
}
