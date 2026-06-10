using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FundiLink.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBursaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bursaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    FundingType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FieldsOfStudy = table.Column<string>(type: "text", nullable: false),
                    MinimumAps = table.Column<int>(type: "integer", nullable: true),
                    MaxHouseholdIncome = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ProvincesEligible = table.Column<string>(type: "text", nullable: false),
                    ApplicationOpenDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApplicationCloseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExternalApplicationUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bursaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BursaryApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    BursaryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DeadlineDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BursaryApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BursaryApplications_Bursaries_BursaryId",
                        column: x => x.BursaryId,
                        principalTable: "Bursaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BursaryApplications_BursaryId",
                table: "BursaryApplications",
                column: "BursaryId");

            migrationBuilder.CreateIndex(
                name: "IX_BursaryApplications_LearnerId",
                table: "BursaryApplications",
                column: "LearnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BursaryApplications");

            migrationBuilder.DropTable(
                name: "Bursaries");
        }
    }
}
