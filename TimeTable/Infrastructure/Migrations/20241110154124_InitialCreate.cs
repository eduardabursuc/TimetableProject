using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        const string ROOMS = "rooms";
        const string CONSTRAINTS = "constraints";
        
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            const string INTEGER = "integer";
            const string CHARACTER_VARYING_200 = "character varying(200)";
            const string CHARACTER_VARYING_50 = "character varying(50)";
            
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    CourseName = table.Column<string>(type: CHARACTER_VARYING_200, maxLength: 200, nullable: false),
                    Credits = table.Column<int>(type: INTEGER, nullable: false),
                    Package = table.Column<string>(type: CHARACTER_VARYING_200, maxLength: 200, nullable: false),
                    Semester = table.Column<int>(type: INTEGER, nullable: false),
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
                    Name = table.Column<string>(type: CHARACTER_VARYING_200, maxLength: 200, nullable: false)
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
                    Name = table.Column<string>(type: CHARACTER_VARYING_200, maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_professors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: ROOMS,
                columns: table => new
                {
                    Name = table.Column<string>(type: CHARACTER_VARYING_200, maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: INTEGER, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: CONSTRAINTS,
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Type = table.Column<int>(type: INTEGER, nullable: false),
                    ProfessorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CourseName = table.Column<string>(type: CHARACTER_VARYING_200, nullable: true),
                    RoomName = table.Column<string>(type: CHARACTER_VARYING_200, nullable: true),
                    WantedRoomName = table.Column<string>(type: CHARACTER_VARYING_200, nullable: true),
                    GroupName = table.Column<string>(type: CHARACTER_VARYING_200, nullable: true),
                    Day = table.Column<string>(type: "text", nullable: true),
                    Time = table.Column<string>(type: "text", nullable: true),
                    WantedDay = table.Column<string>(type: CHARACTER_VARYING_50, maxLength: 50, nullable: true),
                    WantedTime = table.Column<string>(type: CHARACTER_VARYING_50, maxLength: 50, nullable: true),
                    Event = table.Column<string>(type: CHARACTER_VARYING_50, maxLength: 50, nullable: true)
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
                        principalTable: ROOMS,
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_constraints_rooms_WantedRoomName",
                        column: x => x.WantedRoomName,
                        principalTable: ROOMS,
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "timeslots",
                columns: table => new
                {
                    Day = table.Column<string>(type: CHARACTER_VARYING_50, maxLength: 50, nullable: false),
                    Time = table.Column<string>(type: CHARACTER_VARYING_50, maxLength: 50, nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    RoomName = table.Column<string>(type: CHARACTER_VARYING_200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeslots", x => new { x.Time, x.Day });
                    table.ForeignKey(
                        name: "FK_timeslots_rooms_RoomName",
                        column: x => x.RoomName,
                        principalTable: ROOMS,
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_constraints_CourseName",
                table: CONSTRAINTS,
                column: "CourseName");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_GroupName",
                table: CONSTRAINTS,
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_ProfessorId",
                table: CONSTRAINTS,
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_RoomName",
                table: CONSTRAINTS,
                column: "RoomName");

            migrationBuilder.CreateIndex(
                name: "IX_constraints_WantedRoomName",
                table: CONSTRAINTS,
                column: "WantedRoomName");

            migrationBuilder.CreateIndex(
                name: "IX_timeslots_RoomName",
                table: "timeslots",
                column: "RoomName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: CONSTRAINTS);

            migrationBuilder.DropTable(
                name: "timeslots");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "professors");

            migrationBuilder.DropTable(
                name: ROOMS);
        }
    }
}
