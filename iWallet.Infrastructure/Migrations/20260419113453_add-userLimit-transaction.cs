using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iWallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adduserLimittransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLimits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PerTransactionLimit = table.Column<decimal>(type: "decimal(18,2)", maxLength: 5000, nullable: false),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLimits", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLimits");
        }
    }
}
