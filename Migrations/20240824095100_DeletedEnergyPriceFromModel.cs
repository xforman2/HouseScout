using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseScout.Migrations
{
    /// <inheritdoc />
    public partial class DeletedEnergyPriceFromModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnergyPrice",
                table: "Estates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EnergyPrice",
                table: "Estates",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
