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
                    Id = table.Column<string>(type: "text", nullable: false),
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
                name: "timeslots",
                columns: table => new
                {
                    Day = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Time = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeslots", x => new { x.Time, x.Day });
                });

            migrationBuilder.CreateTable(
                name: "CourseProfessor",
                columns: table => new
                {
                    CoursesCourseName = table.Column<string>(type: "character varying(200)", nullable: false),
                    ProfessorsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseProfessor", x => new { x.CoursesCourseName, x.ProfessorsId });
                    table.ForeignKey(
                        name: "FK_CourseProfessor_courses_CoursesCourseName",
                        column: x => x.CoursesCourseName,
                        principalTable: "courses",
                        principalColumn: "CourseName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseProfessor_professors_ProfessorsId",
                        column: x => x.ProfessorsId,
                        principalTable: "professors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "constraints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ProfessorId = table.Column<string>(type: "text", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_CourseProfessor_ProfessorsId",
                table: "CourseProfessor",
                column: "ProfessorsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "constraints");

            migrationBuilder.DropTable(
                name: "CourseProfessor");

            migrationBuilder.DropTable(
                name: "timeslots");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "professors");
        }
    }
}
