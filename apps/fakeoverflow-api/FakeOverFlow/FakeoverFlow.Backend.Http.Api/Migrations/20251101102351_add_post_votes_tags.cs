using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace FakeoverFlow.Backend.Http.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_post_votes_tags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PostContent_PostId_ContentType",
                schema: "public",
                table: "PostContent");

            migrationBuilder.DropColumn(
                name: "Votes",
                schema: "public",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "PostsId",
                schema: "public",
                table: "PostContent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Votes",
                schema: "public",
                table: "PostContent",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Tag",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, defaultValue: ""),
                    Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    VectorText = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Value", "Description" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                schema: "public",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpVote = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => new { x.UserId, x.ContentId });
                    table.ForeignKey(
                        name: "FK_Votes_PostContent_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "public",
                        principalTable: "PostContent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votes_UserAccounts_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostTags",
                schema: "public",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "integer", nullable: false),
                    PostId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTags", x => new { x.PostId, x.TagId });
                    table.ForeignKey(
                        name: "FK_PostTags_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "public",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTags_Tag_TagId",
                        column: x => x.TagId,
                        principalSchema: "public",
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostContent_PostId_ContentType",
                schema: "public",
                table: "PostContent",
                columns: new[] { "PostId", "ContentType" },
                unique: true,
                filter: "\"ContentType\" = 'questions'");

            migrationBuilder.CreateIndex(
                name: "IX_PostContent_PostsId",
                schema: "public",
                table: "PostContent",
                column: "PostsId");

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_TagId",
                schema: "public",
                table: "PostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Value",
                schema: "public",
                table: "Tag",
                column: "Value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tag_VectorText",
                schema: "public",
                table: "Tag",
                column: "VectorText")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ContentId",
                schema: "public",
                table: "Votes",
                column: "ContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostContent_Posts_PostsId",
                schema: "public",
                table: "PostContent",
                column: "PostsId",
                principalSchema: "public",
                principalTable: "Posts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostContent_Posts_PostsId",
                schema: "public",
                table: "PostContent");

            migrationBuilder.DropTable(
                name: "PostTags",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Votes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Tag",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_PostContent_PostId_ContentType",
                schema: "public",
                table: "PostContent");

            migrationBuilder.DropIndex(
                name: "IX_PostContent_PostsId",
                schema: "public",
                table: "PostContent");

            migrationBuilder.DropColumn(
                name: "PostsId",
                schema: "public",
                table: "PostContent");

            migrationBuilder.DropColumn(
                name: "Votes",
                schema: "public",
                table: "PostContent");

            migrationBuilder.AddColumn<long>(
                name: "Votes",
                schema: "public",
                table: "Posts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_PostContent_PostId_ContentType",
                schema: "public",
                table: "PostContent",
                columns: new[] { "PostId", "ContentType" },
                unique: true,
                filter: "\"ContentType\" = 'Questions'");
        }
    }
}
