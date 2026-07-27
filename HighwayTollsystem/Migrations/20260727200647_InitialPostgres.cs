using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HighwayTollsystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TollGates",
                columns: table => new
                {
                    GateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HighwayName = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    KilometerPost = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Direction = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GpsLatitude = table.Column<decimal>(type: "numeric(9,6)", nullable: false),
                    GpsLongitude = table.Column<decimal>(type: "numeric(9,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TollGate__9582C65039CBB277", x => x.GateId);
                });

            migrationBuilder.CreateTable(
                name: "VehicleTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BaseTarif = table.Column<decimal>(type: "numeric(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VehicleT__3214EC0735D28DED", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViolationTypes",
                columns: table => new
                {
                    ViolationTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DefaultPenaltyAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Violatio__3B1A4D1D71E82FAB", x => x.ViolationTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Spz = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TypeId = table.Column<int>(type: "integer", nullable: false),
                    EmissionClass = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vehicles__CA1E142DEAD30959", x => x.Spz);
                    table.ForeignKey(
                        name: "FK__Vehicles__TypeId__5FB337D6",
                        column: x => x.TypeId,
                        principalTable: "VehicleTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Passages",
                columns: table => new
                {
                    PassageId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Spz = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    GateId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VehicleSpeed = table.Column<int>(type: "integer", nullable: false),
                    CalculatedFee = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    IsVignetteValid = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Passages__CC0F002C4A8A415B", x => x.PassageId);
                    table.ForeignKey(
                        name: "FK__Passages__GateId__6D0D32F4",
                        column: x => x.GateId,
                        principalTable: "TollGates",
                        principalColumn: "GateId");
                    table.ForeignKey(
                        name: "FK__Passages__Spz__6C190EBB",
                        column: x => x.Spz,
                        principalTable: "Vehicles",
                        principalColumn: "Spz");
                });

            migrationBuilder.CreateTable(
                name: "Stk",
                columns: table => new
                {
                    StkId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Spz = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmissionsValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Stk__56A80AECC8856588", x => x.StkId);
                    table.ForeignKey(
                        name: "FK__Stk__Spz__693CA210",
                        column: x => x.Spz,
                        principalTable: "Vehicles",
                        principalColumn: "Spz");
                });

            migrationBuilder.CreateTable(
                name: "Vignettes",
                columns: table => new
                {
                    VignetteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Spz = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vignette__C81AEC1FA0C45C6E", x => x.VignetteId);
                    table.ForeignKey(
                        name: "FK__Vignettes__Spz__656C112C",
                        column: x => x.Spz,
                        principalTable: "Vehicles",
                        principalColumn: "Spz");
                });

            migrationBuilder.CreateTable(
                name: "TrafficViolations",
                columns: table => new
                {
                    ViolationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PassageId = table.Column<long>(type: "bigint", nullable: false),
                    ViolationTypeId = table.Column<int>(type: "integer", nullable: false),
                    Details = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActualPenaltyAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TrafficV__18B6DC086635FFDB", x => x.ViolationId);
                    table.ForeignKey(
                        name: "FK__TrafficVi__Passa__72C60C4A",
                        column: x => x.PassageId,
                        principalTable: "Passages",
                        principalColumn: "PassageId");
                    table.ForeignKey(
                        name: "FK__TrafficVi__Viola__73BA3083",
                        column: x => x.ViolationTypeId,
                        principalTable: "ViolationTypes",
                        principalColumn: "ViolationTypeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Passages_GateId",
                table: "Passages",
                column: "GateId");

            migrationBuilder.CreateIndex(
                name: "IX_Passages_Spz",
                table: "Passages",
                column: "Spz");

            migrationBuilder.CreateIndex(
                name: "IX_Stk_Spz",
                table: "Stk",
                column: "Spz");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficViolations_PassageId",
                table: "TrafficViolations",
                column: "PassageId");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficViolations_ViolationTypeId",
                table: "TrafficViolations",
                column: "ViolationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TypeId",
                table: "Vehicles",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vignettes_Spz_Dates",
                table: "Vignettes",
                columns: new[] { "Spz", "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "UQ__Violatio__A25C5AA7571436D1",
                table: "ViolationTypes",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stk");

            migrationBuilder.DropTable(
                name: "TrafficViolations");

            migrationBuilder.DropTable(
                name: "Vignettes");

            migrationBuilder.DropTable(
                name: "Passages");

            migrationBuilder.DropTable(
                name: "ViolationTypes");

            migrationBuilder.DropTable(
                name: "TollGates");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleTypes");
        }
    }
}
