using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class DbCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarPath = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "default.jpg"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Farms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Farms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FarmUser",
                columns: table => new
                {
                    CollaboratorsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FriendsFarmsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmUser", x => new { x.CollaboratorsId, x.FriendsFarmsId });
                    table.ForeignKey(
                        name: "FK_FarmUser_Farms_FriendsFarmsId",
                        column: x => x.FriendsFarmsId,
                        principalTable: "Farms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FarmUser_Users_CollaboratorsId",
                        column: x => x.CollaboratorsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 1, 23, 16, 57, 21, 703, DateTimeKind.Local).AddTicks(3826)),
                    DeathDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HappynessDayCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    HungerLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ThirstyLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Body = table.Column<int>(type: "int", nullable: false),
                    Eye = table.Column<int>(type: "int", nullable: false),
                    Nose = table.Column<int>(type: "int", nullable: false),
                    Mouth = table.Column<int>(type: "int", nullable: false),
                    FarmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pets_Farms_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DrinkingHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DringkingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrinkingHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrinkingHistory_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedingHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeedingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedingHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedingHistory_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrinkingHistory_PetId",
                table: "DrinkingHistory",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Farms_UserId",
                table: "Farms",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FarmUser_FriendsFarmsId",
                table: "FarmUser",
                column: "FriendsFarmsId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedingHistory_PetId",
                table: "FeedingHistory",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_FarmId",
                table: "Pets",
                column: "FarmId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrinkingHistory");

            migrationBuilder.DropTable(
                name: "FarmUser");

            migrationBuilder.DropTable(
                name: "FeedingHistory");

            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "Farms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
