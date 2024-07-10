using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebBanQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class initdb1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "32405586-829f-4466-b286-7d928c6ae71e");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5cdf6f0c-7f43-48a2-912d-6047bd034e9a", "5e583b2d-2b37-4c48-b82a-c0b40744a15c" });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "5cdf6f0c-7f43-48a2-912d-6047bd034e9a");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "5e583b2d-2b37-4c48-b82a-c0b40744a15c");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "OrderInfos",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "OrderInfos");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "32405586-829f-4466-b286-7d928c6ae71e", "1", "Customer", "Customer" },
                    { "5cdf6f0c-7f43-48a2-912d-6047bd034e9a", "1", "Admin", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "5e583b2d-2b37-4c48-b82a-c0b40744a15c", 0, "ced377c5-eb26-4362-b681-617372ac400b", "cskhshop5@gmail.com", true, "Đạo Thanh Hưng", true, false, null, "CSKHSHOP5@GMAIL.COM", "HUNGDTADMIN", "AQAAAAEAACcQAAAAEL5j4uY7WZt+gQxzB/sSfYWx05jZW/HXZDlCuyByPP+3jcmM+5kKM7yQdPpsOR+4VQ==", "0000000000", false, "a6a531b4-5f00-4a06-9a09-a750c089f2c7", false, "hungdtadmin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "5cdf6f0c-7f43-48a2-912d-6047bd034e9a", "5e583b2d-2b37-4c48-b82a-c0b40744a15c" });
        }
    }
}
