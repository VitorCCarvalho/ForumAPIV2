﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumCRUD.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFThreadImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            

            migrationBuilder.CreateTable(
                name: "FThreadImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FThreadId = table.Column<int>(type: "int", nullable: false),
                    ImgId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FThreadImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FThreadImage_threads_Id",
                        column: x => x.Id,
                        principalTable: "threads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "FThreadImage");
        }
    }
}
