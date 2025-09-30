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
                name: "VectorText",
                schema: "public",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "public",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "public",
                table: "UserAccounts");

            migrationBuilder.AddColumn<NpgsqlTypes.NpgsqlTsVector>(
                    name: "VectorText",
                    schema: "public",
                    table: "UserAccounts",
                    type: "tsvector",
                    nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Email", "Username" });

            migrationBuilder.CreateIndex(
                    name: "IX_UserAccounts_VectorText",
                    schema: "public",
                    table: "UserAccounts",
                    column: "VectorText")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VectorText",
                schema: "public",
                table: "UserAccounts");

            // Add back FirstName + LastName
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

            // Recreate VectorText using old definition
            migrationBuilder.AddColumn<NpgsqlTypes.NpgsqlTsVector>(
                    name: "VectorText",
                    schema: "public",
                    table: "UserAccounts",
                    type: "tsvector",
                    nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "FirstName", "LastName", "Email", "Username" });

            migrationBuilder.CreateIndex(
                    name: "IX_UserAccounts_VectorText",
                    schema: "public",
                    table: "UserAccounts",
                    column: "VectorText")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }
    }
}
