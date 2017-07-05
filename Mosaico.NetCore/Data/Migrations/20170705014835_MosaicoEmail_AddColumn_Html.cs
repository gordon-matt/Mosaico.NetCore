using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mosaico.NetCore.Data.Migrations
{
    public partial class MosaicoEmail_AddColumn_Html : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Html",
                table: "MosaicoEmails",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Html",
                table: "MosaicoEmails");
        }
    }
}
