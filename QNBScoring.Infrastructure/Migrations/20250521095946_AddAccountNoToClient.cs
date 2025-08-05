using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QNBScoring.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountNoToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountNo",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNo",
                table: "Clients");
        }
    }
}
