using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebBanQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class addpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Payment_PaymentId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "ae44db63-8aab-44e9-aa14-97231599e442");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "de3a0e9d-0298-49b4-9867-54eb814e790d", "358f0524-0243-48f8-9c3b-09b373f71b8b" });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "de3a0e9d-0298-49b4-9867-54eb814e790d");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "358f0524-0243-48f8-9c3b-09b373f71b8b");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6eaa3a7a-7492-4366-b374-9e6b8a8fd3c0", "1", "Admin", "Admin" },
                    { "8f504eef-0e52-43de-aa70-000613ddb11f", "1", "Customer", "Customer" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "86d88499-5e8b-4865-82b2-e7d87ac64092", 0, "3c00f264-360b-475d-8121-f9124e960cf0", "cskhshop5@gmail.com", true, "Đạo Thanh Hưng", true, false, null, "CSKHSHOP5@GMAIL.COM", "HUNGDTADMIN", "AQAAAAEAACcQAAAAEHOUU9Es7Rd9HACrjt+vORSQUxvU9atMYnwdlzDMThCgcFGjLwf6pYBRyMqtx0zDYQ==", "0000000000", false, "e1225df9-cdd0-4cf8-9556-54b20d4e68eb", false, "hungdtadmin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "6eaa3a7a-7492-4366-b374-9e6b8a8fd3c0", "86d88499-5e8b-4865-82b2-e7d87ac64092" });

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Payments_PaymentId",
                table: "Orders",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Payments_PaymentId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8f504eef-0e52-43de-aa70-000613ddb11f");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "6eaa3a7a-7492-4366-b374-9e6b8a8fd3c0", "86d88499-5e8b-4865-82b2-e7d87ac64092" });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "6eaa3a7a-7492-4366-b374-9e6b8a8fd3c0");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "86d88499-5e8b-4865-82b2-e7d87ac64092");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "ae44db63-8aab-44e9-aa14-97231599e442", "1", "Customer", "Customer" },
                    { "de3a0e9d-0298-49b4-9867-54eb814e790d", "1", "Admin", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "358f0524-0243-48f8-9c3b-09b373f71b8b", 0, "7eef196b-3a19-436f-9501-cd2aebe300a9", "cskhshop5@gmail.com", true, "Đạo Thanh Hưng", true, false, null, "CSKHSHOP5@GMAIL.COM", "HUNGDTADMIN", "AQAAAAEAACcQAAAAENumAnkpu0ZW8t6g6ySMOQgViOepUxpHWA/wln9+d845jrZDjIQ22s16REYaTIGoCg==", "0000000000", false, "6985dfa0-ec57-4327-989b-f2f477f12b76", false, "hungdtadmin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "de3a0e9d-0298-49b4-9867-54eb814e790d", "358f0524-0243-48f8-9c3b-09b373f71b8b" });

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Payment_PaymentId",
                table: "Orders",
                column: "PaymentId",
                principalTable: "Payment",
                principalColumn: "Id");
        }
    }
}
