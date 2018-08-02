using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JwtAuthExample.Migrations
{
    public partial class RemovedUserTokenTableAndAddUserOnTokenTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserToken",
                schema: "users");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                schema: "users",
                table: "Token",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Token_UserId",
                schema: "users",
                table: "Token",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Token_User_UserId",
                schema: "users",
                table: "Token",
                column: "UserId",
                principalSchema: "users",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Token_User_UserId",
                schema: "users",
                table: "Token");

            migrationBuilder.DropIndex(
                name: "IX_Token_UserId",
                schema: "users",
                table: "Token");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "users",
                table: "Token");

            migrationBuilder.CreateTable(
                name: "UserToken",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TokenId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserToken_Token_TokenId",
                        column: x => x.TokenId,
                        principalSchema: "users",
                        principalTable: "Token",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserToken_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "users",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserToken_TokenId",
                schema: "users",
                table: "UserToken",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToken_UserId",
                schema: "users",
                table: "UserToken",
                column: "UserId");
        }
    }
}
