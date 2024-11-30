using System;
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
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    CourseName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Credits = table.Column<int>(type: "integer", nullable: false),
                    Package = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Semester = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courses", x => x.CourseName);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "professors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_professors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "timetables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timetables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "constraints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ProfessorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CourseName = table.Column<string>(type: "character varying(200)", nullable: true),
                    RoomName = table.Column<string>(type: "character varying(200)", nullable: true),
                    WantedRoomName = table.Column<string>(type: "character varying(200)", nullable: true),
                    GroupName = table.Column<string>(type: "character varying(200)", nullable: true),
                    Day = table.Column<string>(type: "text", nullable: true),
                    Time = table.Column<string>(type: "text", nullable: true),
                    WantedDay = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    WantedTime = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Event = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_constraints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_constraints_courses_CourseName",
                        column: x => x.CourseName,
                        principalTable: "courses",
                        principalColumn: "CourseName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_constraints_groups_GroupName",
                        column: x => x.GroupName,
                        principalTable: "groups",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_constraints_professors_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "professors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_constraints_rooms_RoomName",
                        column: x => x.RoomName,
                        principalTable: "rooms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_constraints_rooms_WantedRoomName",
                        column: x => x.WantedRoomName,
                        principalTable: "rooms",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "timeslots",
                columns: table => new
                {
                    TimetableId = table.Column<Guid>(type: "uuid", nullable: false),
                    Day = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<string>(type: "text", nullable: false),
                    RoomName = table.Column<string>(type: "text", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false),
                    EventName = table.Column<string>(type: "text", nullable: false),
                    CourseName = table.Column<string>(type: "text", nullable: false),
                    ProfessorId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeekEvenness = table.Column<bool>(type: "boolean", nullable: false),
                    ProfessorName = table.Column<string>(type: "text", nullable: false),
                    CourseCredits = table.Column<int>(type: "integer", nullable: false),
                    CoursePackage = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeslots", x => new { x.TimetableId, x.Time, x.Day, x.RoomName });
                    table.ForeignKey(
                        name: "FK_timeslots_timetables_TimetableId",
                        column: x => x.TimetableId,
                        principalTable: "timetables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_constraints_CourseName",
                table: "constraints",
                column: "CourseName");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_GroupName",
                table: "constraints",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_ProfessorId",
                table: "constraints",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_RoomName",
                table: "constraints",
                column: "RoomName");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_WantedRoomName",
                table: "constraints",
                column: "WantedRoomName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "constraints");

            migrationBuilder.DropTable(
                name: "timeslots");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "professors");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "timetables");
        }
    }
}
