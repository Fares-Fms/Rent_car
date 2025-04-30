using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test1.Migrations
{
    /// <inheritdoc />
    public partial class t : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // فقط إعادة تسمية العمود
            migrationBuilder.RenameColumn(
                name: "User_ID",
                table: "TbReviews",
                newName: "CarId");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CarId",
                table: "TbReviews",
                newName: "User_ID");
        }

    }
}
