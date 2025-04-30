using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test1.Migrations
{
    /// <inheritdoc />
    public partial class foreginkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         

            // بعدين أضف العمود الجديد Car_ID
            migrationBuilder.AddColumn<int>(
                name: "CarId",
                table: "TbReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // أضف العلاقة مع جدول TbCar
            migrationBuilder.CreateIndex(
                name: "IX_TbReviews_CarId",
                table: "TbReviews",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_TbReviews_TbCar_CarId",
                table: "TbReviews",
                column: "CarId",
                principalTable: "TbCar",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // إلغاء العلاقة الجديدة
            migrationBuilder.DropForeignKey(
                name: "FK_TbReviews_TbCar_CarId",
                table: "TbReviews");

            migrationBuilder.DropIndex(
                name: "IX_TbReviews_CarId",
                table: "TbReviews");

            migrationBuilder.DropColumn(
                name: "CarId",
                table: "TbReviews");

        }

    }
}
