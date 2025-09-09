using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace panda.Migrations
{
    /// <inheritdoc />
    public partial class FixTalentCheckInRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TalentCheckIns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    TalentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WeekStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mood = table.Column<int>(type: "int", nullable: false),
                    Workload = table.Column<int>(type: "int", nullable: false),
                    WhatWentWell = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Blockers = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    HelpNeeded = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    GoalsNextWeek = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TasksDone = table.Column<int>(type: "int", nullable: false),
                    CodeReviewsDone = table.Column<int>(type: "int", nullable: false),
                    MeetingsCount = table.Column<int>(type: "int", nullable: false),
                    MentoringHours = table.Column<double>(type: "float", nullable: false),
                    SkillCSharp = table.Column<int>(type: "int", nullable: false),
                    SkillDotNet = table.Column<int>(type: "int", nullable: false),
                    SkillSql = table.Column<int>(type: "int", nullable: false),
                    SkillFrontend = table.Column<int>(type: "int", nullable: false),
                    SkillCloud = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalentCheckIns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TalentCheckIns_AspNetUsers_TalentId",
                        column: x => x.TalentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TalentCheckIns_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TalentCheckIns_AssignmentId_TalentId_WeekStart",
                table: "TalentCheckIns",
                columns: new[] { "AssignmentId", "TalentId", "WeekStart" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TalentCheckIns_TalentId",
                table: "TalentCheckIns",
                column: "TalentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TalentCheckIns");
        }
    }
}
