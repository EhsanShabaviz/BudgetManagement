using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetRecords",
                columns: table => new
                {
                    SubProjectCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContractTitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContractDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Contractor = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Agent = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ContractStatus = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TotalContractAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    InitialAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    CurrentYearTotalCredit = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    TotalCreditFromStart = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    ExecutiveDept = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    StartDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EndDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExtendedEndDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WorkReferralMethod = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TotalInvoicesAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    TotalWorkProgress = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    CurrentYearInvoicesAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    CreditNumber = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Nature = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AdjustmentAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    MaxRequiredCredit = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    CreditDeficitSupply = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    CreditDeficitCommitment = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetRecords", x => x.SubProjectCode);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetRecords");
        }
    }
}
