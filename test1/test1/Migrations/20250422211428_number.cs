using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test1.Migrations
{
    /// <inheritdoc />
    public partial class number : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "TbReviews");

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "TbReviews",
                type: "nvarchar(10)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "TbReviews");

            migrationBuilder.AddColumn<int>(
                name: "Phone",
                table: "TbReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
