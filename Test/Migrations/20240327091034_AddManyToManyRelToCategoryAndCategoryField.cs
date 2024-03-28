using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test.Migrations
{
    /// <inheritdoc />
    public partial class AddManyToManyRelToCategoryAndCategoryField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CategoryField");

            migrationBuilder.DropColumn(
                name: "Fields",
                table: "Category");

            migrationBuilder.CreateTable(
                name: "CategoryCategoryField",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    FieldsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryCategoryField", x => new { x.CategoriesId, x.FieldsId });
                    table.ForeignKey(
                        name: "FK_CategoryCategoryField_CategoryField_FieldsId",
                        column: x => x.FieldsId,
                        principalTable: "CategoryField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryCategoryField_Category_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCategoryField_FieldsId",
                table: "CategoryCategoryField",
                column: "FieldsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryCategoryField");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "CategoryField",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Fields",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
