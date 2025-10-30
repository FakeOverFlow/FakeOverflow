using System;
using FakeoverFlow.Backend.Http.Api.Models.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace FakeoverFlow.Backend.Http.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_posts_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:content_type", "answers,questions")
                .Annotation("Npgsql:Enum:user_roles", "admin,moderator,user")
                .OldAnnotation("Npgsql:Enum:user_roles", "admin,moderator,user");

            migrationBuilder.CreateTable(
                name: "Posts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Views = table.Column<long>(type: "bigint", nullable: false),
                    Votes = table.Column<long>(type: "bigint", nullable: false),
                    VectorText = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Title" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_UserAccounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "public",
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_UserAccounts_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "public",
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostContent",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<ContentType>(type: "content_type", nullable: false),
                    PostId = table.Column<string>(type: "text", nullable: false),
                    VectorText = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Content" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostContent_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "public",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostContent_UserAccounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "public",
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostContent_UserAccounts_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "public",
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostContent_CreatedBy",
                schema: "public",
                table: "PostContent",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PostContent_PostId_ContentType",
                schema: "public",
                table: "PostContent",
                columns: new[] { "PostId", "ContentType" },
                unique: true,
                filter: "\"ContentType\" = 'questions'");

            migrationBuilder.CreateIndex(
                name: "IX_PostContent_UpdatedBy",
                schema: "public",
                table: "PostContent",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PostContent_VectorText",
                schema: "public",
                table: "PostContent",
                column: "VectorText")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatedBy",
                schema: "public",
                table: "Posts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UpdatedBy",
                schema: "public",
                table: "Posts",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_VectorText",
                schema: "public",
                table: "Posts",
                column: "VectorText")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostContent",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:user_roles", "admin,moderator,user")
                .OldAnnotation("Npgsql:Enum:content_type", "answers,questions")
                .OldAnnotation("Npgsql:Enum:user_roles", "admin,moderator,user");
        }
    }
}
