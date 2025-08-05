using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QNBScoring.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStatutToDemandeAndRenameScoreValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Valeur",
                table: "Scores",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCalcul",
                table: "Scores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AgentResponsable",
                table: "Historiques",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Historiques",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Raison",
                table: "Historiques",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Statut",
                table: "Historiques",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ScoreId",
                table: "Demandes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Statut",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCalcul",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "AgentResponsable",
                table: "Historiques");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Historiques");

            migrationBuilder.DropColumn(
                name: "Raison",
                table: "Historiques");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Historiques");

            migrationBuilder.DropColumn(
                name: "ScoreId",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Demandes");

            migrationBuilder.AlterColumn<double>(
                name: "Valeur",
                table: "Scores",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
