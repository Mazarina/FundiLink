using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FundiLink.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLearnerAndAcademicProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Learners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    IdNumber = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    PassportNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    HomeLanguage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Nationality = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MobileNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Province = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Municipality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Suburb = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SchoolName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SchoolProvince = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GradeLevel = table.Column<string>(type: "text", nullable: false),
                    GuardianName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GuardianPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    GuardianEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ConsentAccepted = table.Column<bool>(type: "boolean", nullable: false),
                    ConsentTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConsentVersion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    GuardianConsentAccepted = table.Column<bool>(type: "boolean", nullable: true),
                    GuardianConsentTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProfileCompleteness = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Learners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AcademicProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    ResultType = table.Column<string>(type: "text", nullable: false),
                    ApsScore = table.Column<int>(type: "integer", nullable: false),
                    ApsCalculatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicProfiles_Learners_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "Learners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NscSubjectResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SubjectCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Percentage = table.Column<int>(type: "integer", nullable: false),
                    ApsPoints = table.Column<int>(type: "integer", nullable: false),
                    IsHomeLanguage = table.Column<bool>(type: "boolean", nullable: false),
                    IsLifeOrientation = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NscSubjectResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NscSubjectResults_AcademicProfiles_AcademicProfileId",
                        column: x => x.AcademicProfileId,
                        principalTable: "AcademicProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicProfiles_LearnerId",
                table: "AcademicProfiles",
                column: "LearnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Learners_UserId",
                table: "Learners",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NscSubjectResults_AcademicProfileId",
                table: "NscSubjectResults",
                column: "AcademicProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NscSubjectResults");

            migrationBuilder.DropTable(
                name: "AcademicProfiles");

            migrationBuilder.DropTable(
                name: "Learners");
        }
    }
}
