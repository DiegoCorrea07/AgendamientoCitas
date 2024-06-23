using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class Ahoraaa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DiaSemana",
                table: "HorariosMedicos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DiaSemana",
                table: "HorariosMedicos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

    }
}
