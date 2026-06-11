using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FundiLink.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGuardianConsentAndCoAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuardianConsents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Scope = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    GuardianName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GuardianContact = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuardianConsents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuardianLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    GuardianUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    GuardianName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GuardianContact = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuardianLinks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuardianConsents_LearnerId_ConsentType_RecordedAt",
                table: "GuardianConsents",
                columns: new[] { "LearnerId", "ConsentType", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GuardianLinks_GuardianUserId_LearnerId",
                table: "GuardianLinks",
                columns: new[] { "GuardianUserId", "LearnerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuardianConsents");

            migrationBuilder.DropTable(
                name: "GuardianLinks");
        }
    }
}
