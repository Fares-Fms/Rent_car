using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test1.Migrations
{
    /// <inheritdoc />
    public partial class f : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbAds");

            migrationBuilder.DropTable(
                name: "TbBrands");

            migrationBuilder.DropTable(
                name: "TbCar");

            migrationBuilder.DropTable(
                name: "TbReviews");

            migrationBuilder.DropTable(
                name: "TbSettings");
        }
    }
}
