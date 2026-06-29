using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEShop.Migrations
{
    /// <inheritdoc />
    public partial class AddMAymentToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Authority",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionCode",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

      
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Authority",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "TransactionCode",
                table: "orders");
        }
    }
}
