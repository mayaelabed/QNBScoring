using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QNBScoring.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DemandeHistorique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Clients_ClientId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ClientId",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNo",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Clients_AccountNo",
                table: "Clients",
                column: "AccountNo");

            migrationBuilder.CreateTable(
                name: "Historiques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Utilisateur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DemandeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historiques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Historiques_Demandes_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "Demandes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountNo",
                table: "Transactions",
                column: "AccountNo");

            migrationBuilder.CreateIndex(
                name: "IX_Historiques_DemandeId",
                table: "Historiques",
                column: "DemandeId");

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

            migrationBuilder.DropTable(
                name: "Historiques");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AccountNo",
                table: "Transactions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Clients_AccountNo",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNo",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ClientId",
                table: "Transactions",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Clients_ClientId",
                table: "Transactions",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }
    }
}
