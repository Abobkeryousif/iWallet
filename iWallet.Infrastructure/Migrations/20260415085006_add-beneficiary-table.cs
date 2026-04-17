using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iWallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addbeneficiarytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beneficiaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    WalletNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_Name",
                table: "Beneficiaries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_WalletNumber",
                table: "Beneficiaries",
                column: "WalletNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beneficiaries");
        }
    }
}
