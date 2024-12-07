using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_constraints_courses_CourseName",
                table: "constraints");

            migrationBuilder.DropForeignKey(
                name: "FK_constraints_groups_GroupName",
                table: "constraints");

            migrationBuilder.DropForeignKey(
                name: "FK_constraints_rooms_RoomName",
                table: "constraints");

            migrationBuilder.DropForeignKey(
                name: "FK_constraints_rooms_WantedRoomName",
                table: "constraints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_rooms",
                table: "rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_groups",
                table: "groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_courses",
                table: "courses");

            migrationBuilder.DropIndex(
                name: "IX_constraints_CourseName",
                table: "constraints");

            migrationBuilder.DropIndex(
                name: "IX_constraints_GroupName",
                table: "constraints");

            migrationBuilder.DropIndex(
                name: "IX_constraints_RoomName",
                table: "constraints");

            migrationBuilder.DropIndex(
                name: "IX_constraints_WantedRoomName",
                table: "constraints");

            migrationBuilder.DropColumn(
                name: "CourseName",
                table: "constraints");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "constraints");

            migrationBuilder.DropColumn(
                name: "RoomName",
                table: "constraints");

            migrationBuilder.DropColumn(
                name: "WantedRoomName",
                table: "constraints");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "timetables",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "timeslots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "timeslots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "timeslots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "rooms",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "rooms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "professors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "groups",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "groups",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "courses",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "courses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "constraints",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "constraints",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "constraints",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TimetableId",
                table: "constraints",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WantedRoomId",
                table: "constraints",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_rooms",
                table: "rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_groups",
                table: "groups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_courses",
                table: "courses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_CourseId",
                table: "constraints",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_GroupId",
                table: "constraints",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_RoomId",
                table: "constraints",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_WantedRoomId",
                table: "constraints",
                column: "WantedRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_constraints_courses_CourseId",
                table: "constraints",
                column: "CourseId",
                principalTable: "courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_constraints_groups_GroupId",
                table: "constraints",
                column: "GroupId",
                principalTable: "groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_constraints_rooms_RoomId",
                table: "constraints",
                column: "RoomId",
                principalTable: "rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_constraints_rooms_WantedRoomId",
                table: "constraints",
                column: "WantedRoomId",
                principalTable: "rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_constraints_courses_CourseId",
                table: "constraints");

            migrationBuilder.DropForeignKey(
                name: "FK_constraints_groups_GroupId",
                table: "constraints");

            migrationBuilder.DropForeignKey(
                name: "FK_constraints_rooms_RoomId",
                table: "constraints");

            migrationBuilder.DropForeignKey(
                name: "FK_constraints_rooms_WantedRoomId",
                table: "constraints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_rooms",
                table: "rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_groups",
                table: "groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_courses",
                table: "courses");

            migrationBuilder.DropIndex(
                name: "IX_constraints_CourseId",
                table: "constraints");

            migrationBuilder.DropIndex(
                name: "IX_constraints_GroupId",
                table: "constraints");

            migrationBuilder.DropIndex(
                name: "IX_constraints_RoomId",
                table: "constraints");

            migrationBuilder.DropIndex(
                name: "IX_constraints_WantedRoomId",
                table: "constraints");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "timetables");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "timeslots");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "timeslots");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "timeslots");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "professors");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "constraints");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "constraints");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "constraints");

            migrationBuilder.DropColumn(
                name: "TimetableId",
                table: "constraints");

            migrationBuilder.DropColumn(
                name: "WantedRoomId",
                table: "constraints");

            migrationBuilder.AddColumn<string>(
                name: "CourseName",
                table: "constraints",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "constraints",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomName",
                table: "constraints",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WantedRoomName",
                table: "constraints",
                type: "character varying(200)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_rooms",
                table: "rooms",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_groups",
                table: "groups",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_courses",
                table: "courses",
                column: "CourseName");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_CourseName",
                table: "constraints",
                column: "CourseName");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_GroupName",
                table: "constraints",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_RoomName",
                table: "constraints",
                column: "RoomName");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_WantedRoomName",
                table: "constraints",
                column: "WantedRoomName");

            migrationBuilder.AddForeignKey(
                name: "FK_constraints_courses_CourseName",
                table: "constraints",
                column: "CourseName",
                principalTable: "courses",
                principalColumn: "CourseName",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_constraints_groups_GroupName",
                table: "constraints",
                column: "GroupName",
                principalTable: "groups",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_constraints_rooms_RoomName",
                table: "constraints",
                column: "RoomName",
                principalTable: "rooms",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_constraints_rooms_WantedRoomName",
                table: "constraints",
                column: "WantedRoomName",
                principalTable: "rooms",
                principalColumn: "Name");
        }
    }
}
