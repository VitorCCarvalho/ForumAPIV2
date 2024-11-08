using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumCRUD.API.Migrations
{
    /// <inheritdoc />
    public partial class FixFThreadImageFthreadIdFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

   
            migrationBuilder.AddPrimaryKey(
                name: "PK_fthreadimage",
                table: "fthreadimage",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_fthreadimage_FThreadId",
                table: "fthreadimage",
                column: "FThreadId");

            migrationBuilder.AddForeignKey(
                name: "FK_fthreadimage_threads_FThreadId",
                table: "fthreadimage",
                column: "FThreadId",
                principalTable: "threads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddPrimaryKey(
                name: "PK_FThreadImage",
                table: "FThreadImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FThreadImage_threads_Id",
                table: "FThreadImage",
                column: "Id",
                principalTable: "threads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
