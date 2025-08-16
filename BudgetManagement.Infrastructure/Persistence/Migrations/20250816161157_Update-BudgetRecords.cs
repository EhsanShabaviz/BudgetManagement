using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBudgetRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Increase25Percent",
                table: "BudgetRecords");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Increase25Percent",
                table: "BudgetRecords",
                type: "decimal(18,0)",
                nullable: true);
        }
    }
}
