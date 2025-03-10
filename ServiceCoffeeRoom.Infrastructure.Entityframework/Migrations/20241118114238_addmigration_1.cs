using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ServiceCoffeeRoom.Infrastructure.Entityframework.Migrations
{
    /// <inheritdoc />
    public partial class addmigration_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BeansBags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Mark = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    RemainingWeight = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    TimeHappened = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeansBags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    VisitorId = table.Column<long>(type: "bigint", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeHappened = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    VisitorId = table.Column<long>(type: "bigint", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeHappened = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    TelegramAccaunt = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    IsUser = table.Column<bool>(type: "boolean", nullable: false),
                    CashAccount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VisitorId = table.Column<long>(type: "bigint", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeHappened = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoffeeMachines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CurrenBeansId = table.Column<Guid>(type: "uuid", nullable: false),
                    BeansId = table.Column<Guid>(type: "uuid", nullable: true),
                    CountCupAll = table.Column<int>(type: "integer", nullable: false),
                    CountCupService = table.Column<int>(type: "integer", nullable: false),
                    LimitService = table.Column<int>(type: "integer", nullable: false),
                    SizeOfOneCup = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoffeeMachines_BeansBags_BeansId",
                        column: x => x.BeansId,
                        principalTable: "BeansBags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AdminId = table.Column<long>(type: "bigint", nullable: false),
                    CoffeeMachineId = table.Column<Guid>(type: "uuid", nullable: false),
                    PriceService = table.Column<int>(type: "integer", nullable: false),
                    Bank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_CoffeeMachines_CoffeeMachineId",
                        column: x => x.CoffeeMachineId,
                        principalTable: "CoffeeMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rooms_Persons_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeMachines_BeansId",
                table: "CoffeeMachines",
                column: "BeansId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AdminId",
                table: "Rooms",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CoffeeMachineId",
                table: "Rooms",
                column: "CoffeeMachineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cups");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "CoffeeMachines");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "BeansBags");
        }
    }
}
