using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FundiLink.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccommodationAndCareerOpportunities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccommodationListings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    AccommodationType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    NearInstitution = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IndicativeMonthlyCost = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ContactUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationListings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CareerOpportunities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    OpportunityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FieldsOfInterest = table.Column<string>(type: "text", nullable: false),
                    MinimumGradeLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProvincesEligible = table.Column<string>(type: "text", nullable: false),
                    ApplicationCloseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExternalApplicationUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareerOpportunities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccommodationInterests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccommodationListingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccommodationInterests_AccommodationListings_AccommodationL~",
                        column: x => x.AccommodationListingId,
                        principalTable: "AccommodationListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CareerInterests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CareerOpportunityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareerInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CareerInterests_CareerOpportunities_CareerOpportunityId",
                        column: x => x.CareerOpportunityId,
                        principalTable: "CareerOpportunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationInterests_AccommodationListingId",
                table: "AccommodationInterests",
                column: "AccommodationListingId");

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationInterests_LearnerId_AccommodationListingId",
                table: "AccommodationInterests",
                columns: new[] { "LearnerId", "AccommodationListingId" });

            migrationBuilder.CreateIndex(
                name: "IX_CareerInterests_CareerOpportunityId",
                table: "CareerInterests",
                column: "CareerOpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CareerInterests_LearnerId_CareerOpportunityId",
                table: "CareerInterests",
                columns: new[] { "LearnerId", "CareerOpportunityId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccommodationInterests");

            migrationBuilder.DropTable(
                name: "CareerInterests");

            migrationBuilder.DropTable(
                name: "AccommodationListings");

            migrationBuilder.DropTable(
                name: "CareerOpportunities");
        }
    }
}
