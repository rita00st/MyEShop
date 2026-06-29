using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEShop.Migrations
{
    /// <inheritdoc />
    public partial class AddmoreOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<long>(
                name: "RefId",
                table: "orders",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefId",
                table: "orders");

        }
    }
}
