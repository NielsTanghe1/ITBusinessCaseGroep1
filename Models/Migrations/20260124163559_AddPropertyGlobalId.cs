using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyGlobalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GlobalId",
                table: "PaymentDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GlobalId",
                table: "Orders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GlobalId",
                table: "OrderItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GlobalId",
                table: "Coffees",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GlobalId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GlobalId",
                table: "Addresses",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GlobalId",
                table: "PaymentDetails");

            migrationBuilder.DropColumn(
                name: "GlobalId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GlobalId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "GlobalId",
                table: "Coffees");

            migrationBuilder.DropColumn(
                name: "GlobalId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GlobalId",
                table: "Addresses");
        }
    }
}
