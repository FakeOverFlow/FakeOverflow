using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeoverFlow.Backend.Http.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_analysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:content_type", "analysis,answers,questions")
                .Annotation("Npgsql:Enum:user_roles", "admin,moderator,user")
                .OldAnnotation("Npgsql:Enum:content_type", "answers,questions")
                .OldAnnotation("Npgsql:Enum:user_roles", "admin,moderator,user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:content_type", "answers,questions")
                .Annotation("Npgsql:Enum:user_roles", "admin,moderator,user")
                .OldAnnotation("Npgsql:Enum:content_type", "analysis,answers,questions")
                .OldAnnotation("Npgsql:Enum:user_roles", "admin,moderator,user");
        }
    }
}
