using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QNBScoring.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRaisonAndTransactionReferenceToDemande : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Raison",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TransactionReference",
                table: "Demandes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Raison",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "TransactionReference",
                table: "Demandes");
        }
    }
}
