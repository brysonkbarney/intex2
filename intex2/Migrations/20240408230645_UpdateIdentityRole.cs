using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace intex2.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentityRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: " LineItems");

            migrationBuilder.DropTable(
                name: " Orders");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
