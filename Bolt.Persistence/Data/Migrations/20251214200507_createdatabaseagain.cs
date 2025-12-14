using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bolt.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class createdatabaseagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DestinationAddress_City",
                table: "RideOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationAddress_Location_Latitude",
                table: "RideOrders",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DestinationAddress_Location_Longitude",
                table: "RideOrders",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DestinationAddress_PostalCode",
                table: "RideOrders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationAddress_Street",
                table: "RideOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedFare_Amount",
                table: "RideOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "EstimatedFare_Currency",
                table: "RideOrders",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "FinalFare_Amount",
                table: "RideOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalFare_Currency",
                table: "RideOrders",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupAddress_City",
                table: "RideOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PickupAddress_Location_Latitude",
                table: "RideOrders",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PickupAddress_Location_Longitude",
                table: "RideOrders",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PickupAddress_PostalCode",
                table: "RideOrders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupAddress_Street",
                table: "RideOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Rating_TotalReviews",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Rating_Value",
                table: "Reviews",
                type: "decimal(3,1)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationAddress_City",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "DestinationAddress_Location_Latitude",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "DestinationAddress_Location_Longitude",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "DestinationAddress_PostalCode",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "DestinationAddress_Street",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "EstimatedFare_Amount",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "EstimatedFare_Currency",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "FinalFare_Amount",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "FinalFare_Currency",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "PickupAddress_City",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "PickupAddress_Location_Latitude",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "PickupAddress_Location_Longitude",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "PickupAddress_PostalCode",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "PickupAddress_Street",
                table: "RideOrders");

            migrationBuilder.DropColumn(
                name: "Rating_TotalReviews",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Rating_Value",
                table: "Reviews");
        }
    }
}
