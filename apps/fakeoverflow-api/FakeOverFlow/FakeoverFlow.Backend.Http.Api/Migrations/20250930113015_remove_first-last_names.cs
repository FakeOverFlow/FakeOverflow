using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace FakeoverFlow.Backend.Http.Api.Migrations
{
    /// <inheritdoc />
    public partial class remove_firstlast_names : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "public",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "public",
                table: "UserAccounts");

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "VectorText",
                schema: "public",
                table: "UserAccounts",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Email", "Username" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "FirstName", "LastName", "Email", "Username" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "VectorText",
                schema: "public",
                table: "UserAccounts",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "FirstName", "LastName", "Email", "Username" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "Email", "Username" });

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "public",
                table: "UserAccounts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "public",
                table: "UserAccounts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
