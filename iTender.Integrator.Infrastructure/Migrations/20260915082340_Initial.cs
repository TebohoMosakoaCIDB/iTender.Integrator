using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iTender.Integrator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Releases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ocid = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReleaseId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    InitiationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuyerId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BuyerName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FetchedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSyncedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Awards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Value_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Value_Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Awards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Awards_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AwardExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Period_StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Period_EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Period_MaxExtentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Period_DurationInDays = table.Column<int>(type: "int", nullable: true),
                    Value_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Value_Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DateSigned = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComplianceStatus = table.Column<int>(type: "int", nullable: false),
                    LastComplianceCheckUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegistrationScheme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address_StreetAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Address_Locality = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address_Region = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address_PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address_CountryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPoint_Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPoint_Telephone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactPoint_Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPoint_FaxNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactPoint_Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Roles = table.Column<int>(type: "int", nullable: false),
                    ContractorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parties_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tenders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MainProcurementCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AdditionalProcurementCategories = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveryLocation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EligibilityCriteria = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProcurementMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProcurementMethodDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProcuringEntityId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProcuringEntityName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Classification_Scheme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Classification_Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Classification_Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Value_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Value_Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TenderPeriod_StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenderPeriod_EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenderPeriod_MaxExtentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenderPeriod_DurationInDays = table.Column<int>(type: "int", nullable: true),
                    EnquiryPeriod_StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EnquiryPeriod_EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EnquiryPeriod_MaxExtentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EnquiryPeriod_DurationInDays = table.Column<int>(type: "int", nullable: true),
                    AwardPeriod_StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AwardPeriod_EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AwardPeriod_MaxExtentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AwardPeriod_DurationInDays = table.Column<int>(type: "int", nullable: true),
                    ContactPerson_Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPerson_Telephone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactPerson_Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPerson_FaxNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactPerson_Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BriefingSession_IsSession = table.Column<bool>(type: "bit", nullable: true),
                    BriefingSession_Compulsory = table.Column<bool>(type: "bit", nullable: true),
                    BriefingSession_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BriefingSession_Venue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tenders_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AwardSuppliers",
                columns: table => new
                {
                    AwardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AwardSuppliers", x => new { x.AwardId, x.Id });
                    table.ForeignKey(
                        name: "FK_AwardSuppliers_Awards_AwardId",
                        column: x => x.AwardId,
                        principalTable: "Awards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractMilestones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateMet = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractMilestones_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractTransactions",
                columns: table => new
                {
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Value_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Value_Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PayerId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PayeeId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractTransactions", x => new { x.ContractId, x.Id });
                    table.ForeignKey(
                        name: "FK_ContractTransactions_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Value_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Value_Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ContractPeriod_StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractPeriod_EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractPeriod_MaxExtentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractPeriod_DurationInDays = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasRenewal = table.Column<bool>(type: "bit", nullable: false),
                    HasOptions = table.Column<bool>(type: "bit", nullable: false),
                    TenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lots_Tenders_TenderId",
                        column: x => x.TenderId,
                        principalTable: "Tenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenderDocument",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DatePublished = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Format = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderDocument_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TenderDocument_Tenders_TenderId",
                        column: x => x.TenderId,
                        principalTable: "Tenders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TenderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Classification_Scheme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Classification_Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Classification_Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderItems_Tenders_TenderId",
                        column: x => x.TenderId,
                        principalTable: "Tenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Awards_ReleaseId",
                table: "Awards",
                column: "ReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractMilestones_ContractId",
                table: "ContractMilestones",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_AwardExternalId",
                table: "Contracts",
                column: "AwardExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ReleaseId",
                table: "Contracts",
                column: "ReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_TenderId",
                table: "Lots",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_RegistrationNumber",
                table: "Parties",
                column: "RegistrationNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_ReleaseId",
                table: "Parties",
                column: "ReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_LastSyncedAtUtc",
                table: "Releases",
                column: "LastSyncedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_Ocid_ReleaseId",
                table: "Releases",
                columns: new[] { "Ocid", "ReleaseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenderDocument_ContractId",
                table: "TenderDocument",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderDocument_TenderId",
                table: "TenderDocument",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderItems_TenderId",
                table: "TenderItems",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenders_ExternalId",
                table: "Tenders",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenders_ReleaseId",
                table: "Tenders",
                column: "ReleaseId",
                unique: true,
                filter: "[ReleaseId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AwardSuppliers");

            migrationBuilder.DropTable(
                name: "ContractMilestones");

            migrationBuilder.DropTable(
                name: "ContractTransactions");

            migrationBuilder.DropTable(
                name: "Lots");

            migrationBuilder.DropTable(
                name: "Parties");

            migrationBuilder.DropTable(
                name: "TenderDocument");

            migrationBuilder.DropTable(
                name: "TenderItems");

            migrationBuilder.DropTable(
                name: "Awards");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Tenders");

            migrationBuilder.DropTable(
                name: "Releases");
        }
    }
}
