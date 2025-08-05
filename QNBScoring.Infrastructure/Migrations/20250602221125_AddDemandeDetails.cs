using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QNBScoring.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDemandeDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdresseLivraison",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ConditionsAcceptees",
                table: "Demandes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DeclarationVeracite",
                table: "Demandes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JustificatifDomicilePath",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModeLivraison",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Motif",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NombreChequiers",
                table: "Demandes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PieceIdentitePath",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeChequier",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdresseLivraison",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "ConditionsAcceptees",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "DeclarationVeracite",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "JustificatifDomicilePath",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "ModeLivraison",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "Motif",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "NombreChequiers",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "PieceIdentitePath",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "TypeChequier",
                table: "Demandes");
        }
    }
}
