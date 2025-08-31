using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BudgetManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNatureTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Natures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Natures", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Natures",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "آبرسانی" },
                    { 2, "آبهای سطحی" },
                    { 3, "آسفالت" },
                    { 4, "اصلاح هندسی" },
                    { 5, "ایستگاه اتوبوس" },
                    { 6, "بازسازی ساختمان" },
                    { 7, "بهسازی پیاده رو" },
                    { 8, "بهسازی جداره" },
                    { 9, "تجهیزات ترافیکی" },
                    { 10, "ساماندهی معابر " },
                    { 11, "علائم افقی ترافيکی" },
                    { 12, "فضاهای بی دفاع" },
                    { 13, "مخزن بتنی" },
                    { 14, "نگهداری ساختمان" },
                    { 15, "نگهداری فضای سبز" },
                    { 16, "نگهداری قنوات" },
                    { 17, "نورپردازی" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Natures");
        }
    }
}
