using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class renamedrecordtablesandaddeduniqueindexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrinkingHistory");

            migrationBuilder.DropTable(
                name: "FeedingHistory");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Pets",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Farms",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "HungryStateChangesHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangesDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFeeding = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HungryStateChangesHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HungryStateChangesHistory_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThirstyStateChangesHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangesDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDrinking = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThirstyStateChangesHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThirstyStateChangesHistory_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pets_Name",
                table: "Pets",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Farms_Name",
                table: "Farms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HungryStateChangesHistory_PetId",
                table: "HungryStateChangesHistory",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_ThirstyStateChangesHistory_PetId",
                table: "ThirstyStateChangesHistory",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HungryStateChangesHistory");

            migrationBuilder.DropTable(
                name: "ThirstyStateChangesHistory");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Pets_Name",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_Farms_Name",
                table: "Farms");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Farms",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "DrinkingHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DringkingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    PetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeedingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_FeedingHistory_PetId",
                table: "FeedingHistory",
                column: "PetId");
        }
    }
}
