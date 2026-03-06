using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IIS_backend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Competitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DiscountValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrencyName = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UsageDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketCreationRequestErrors",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    TicketCode = table.Column<string>(type: "text", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCreationRequestErrors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Days",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    CompetitionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Days", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Days_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Characteristics = table.Column<string>(type: "text", nullable: false),
                    PriceAddon = table.Column<decimal>(type: "numeric", nullable: false),
                    CompetitionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zones_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompetitionCurrencies",
                columns: table => new
                {
                    CompetitionId = table.Column<long>(type: "bigint", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionCurrencies", x => new { x.CompetitionId, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_CompetitionCurrencies_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompetitionCurrencies_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentSnapshots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExchangeRate = table.Column<decimal>(type: "numeric", nullable: false),
                    FinalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    SelectedCurrencyId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentSnapshots_Currencies_SelectedCurrencyId",
                        column: x => x.SelectedCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    AddressLine = table.Column<string>(type: "text", nullable: false),
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TicketCode = table.Column<string>(type: "text", nullable: false),
                    PaymentSnapshotId = table.Column<long>(type: "bigint", nullable: false),
                    IssuedPromoCodeId = table.Column<long>(type: "bigint", nullable: false),
                    UsedPromoCodeId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_PaymentSnapshots_PaymentSnapshotId",
                        column: x => x.PaymentSnapshotId,
                        principalTable: "PaymentSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_PromoCodes_IssuedPromoCodeId",
                        column: x => x.IssuedPromoCodeId,
                        principalTable: "PromoCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_PromoCodes_UsedPromoCodeId",
                        column: x => x.UsedPromoCodeId,
                        principalTable: "PromoCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketId = table.Column<long>(type: "bigint", nullable: false),
                    DayId = table.Column<long>(type: "bigint", nullable: false),
                    ZoneId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketItems_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketItems_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketItems_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionCurrencies_CurrencyId",
                table: "CompetitionCurrencies",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CurrencyName",
                table: "Currencies",
                column: "CurrencyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Days_CompetitionId_Date",
                table: "Days",
                columns: new[] { "CompetitionId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSnapshots_SelectedCurrencyId",
                table: "PaymentSnapshots",
                column: "SelectedCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketCreationRequestErrors_Email_TicketCode",
                table: "TicketCreationRequestErrors",
                columns: new[] { "Email", "TicketCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketItems_DayId",
                table: "TicketItems",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketItems_TicketId_DayId",
                table: "TicketItems",
                columns: new[] { "TicketId", "DayId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketItems_ZoneId",
                table: "TicketItems",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IssuedPromoCodeId",
                table: "Tickets",
                column: "IssuedPromoCodeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PaymentSnapshotId",
                table: "Tickets",
                column: "PaymentSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketCode",
                table: "Tickets",
                column: "TicketCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UsedPromoCodeId",
                table: "Tickets",
                column: "UsedPromoCodeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zones_CompetitionId_Characteristics",
                table: "Zones",
                columns: new[] { "CompetitionId", "Characteristics" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompetitionCurrencies");

            migrationBuilder.DropTable(
                name: "TicketCreationRequestErrors");

            migrationBuilder.DropTable(
                name: "TicketItems");

            migrationBuilder.DropTable(
                name: "Days");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.DropTable(
                name: "PaymentSnapshots");

            migrationBuilder.DropTable(
                name: "PromoCodes");

            migrationBuilder.DropTable(
                name: "Competitions");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
