using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ufl_erp_api.Migrations
{
    /// <inheritdoc />
    public partial class ready : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatergoryId",
                table: "Brands",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatergoryId",
                table: "Brands");
        }
    }
}
