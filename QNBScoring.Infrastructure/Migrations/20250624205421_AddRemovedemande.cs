using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QNBScoring.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRemovedemande : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNo",
                table: "Demandes");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNo",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNo",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Clients_AccountNo",
                table: "Clients",
                column: "AccountNo");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountNo",
                table: "Transactions",
                column: "AccountNo");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AccountNo",
                table: "Clients",
                column: "AccountNo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Clients_AccountNo",
                table: "Transactions",
                column: "AccountNo",
                principalTable: "Clients",
                principalColumn: "AccountNo",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Clients_AccountNo",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AccountNo",
                table: "Transactions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Clients_AccountNo",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_AccountNo",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNo",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "AccountNo",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNo",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
