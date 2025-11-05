using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodsStore.Migrations
{
    public partial class AddDescriptionToCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId1",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_SubCategoryId1",
                table: "Items",
                column: "SubCategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_SubCategories_SubCategoryId1",
                table: "Items",
                column: "SubCategoryId1",
                principalTable: "SubCategories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_SubCategories_SubCategoryId1",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_SubCategoryId1",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SubCategoryId1",
                table: "Items");
        }
    }
}
