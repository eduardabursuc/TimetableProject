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
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CourseName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Credits = table.Column<int>(type: "integer", nullable: false),
                    Package = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Semester = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.Id);
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseProfessor",
                columns: table => new
                {
                    CoursesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfessorsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseProfessor", x => new { x.CoursesId, x.ProfessorsId });
                    table.ForeignKey(
                        name: "FK_CourseProfessor_courses_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "courses",
                        principalColumn: "Id",
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
                    ProfessorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    WantedRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    WantedTimeslotId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    @event = table.Column<string>(name: "event", type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_constraints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_constraints_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_constraints_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_constraints_professors_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "professors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_constraints_rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_constraints_rooms_WantedRoomId",
                        column: x => x.WantedRoomId,
                        principalTable: "rooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "timeslots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Day = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Time = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ConstraintId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeslots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_timeslots_constraints_ConstraintId",
                        column: x => x.ConstraintId,
                        principalTable: "constraints",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_constraints_CourseId",
                table: "constraints",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_GroupId",
                table: "constraints",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_ProfessorId",
                table: "constraints",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_RoomId",
                table: "constraints",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_WantedRoomId",
                table: "constraints",
                column: "WantedRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_WantedTimeslotId",
                table: "constraints",
                column: "WantedTimeslotId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseProfessor_ProfessorsId",
                table: "CourseProfessor",
                column: "ProfessorsId");

            migrationBuilder.CreateIndex(
                name: "IX_timeslots_ConstraintId",
                table: "timeslots",
                column: "ConstraintId");

            migrationBuilder.AddForeignKey(
                name: "FK_constraints_timeslots_WantedTimeslotId",
                table: "constraints",
                column: "WantedTimeslotId",
                principalTable: "timeslots",
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
                name: "FK_constraints_professors_ProfessorId",
                table: "constraints");

            migrationBuilder.DropForeignKey(
                name: "FK_constraints_rooms_RoomId",
                table: "constraints");

            migrationBuilder.DropForeignKey(
                name: "FK_constraints_rooms_WantedRoomId",
                table: "constraints");

            migrationBuilder.DropForeignKey(
                name: "FK_constraints_timeslots_WantedTimeslotId",
                table: "constraints");

            migrationBuilder.DropTable(
                name: "CourseProfessor");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "professors");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "timeslots");

            migrationBuilder.DropTable(
                name: "constraints");
        }
    }
}
