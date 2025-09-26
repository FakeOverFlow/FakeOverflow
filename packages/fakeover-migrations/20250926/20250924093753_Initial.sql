CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TYPE user_roles AS ENUM ('admin', 'moderator', 'user');

CREATE TABLE "UserAccounts" (
    "Id" uuid NOT NULL,
    "CreatedBy" uuid NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedBy" uuid NOT NULL,
    "UpdatedOn" timestamp with time zone NOT NULL,
    "IsDeleted" boolean NOT NULL DEFAULT FALSE,
    "VectorText" tsvector GENERATED ALWAYS AS (to_tsvector('english', "FirstName" || ' ' || "LastName" || ' ' || "Email" || ' ' || "Username")) STORED NOT NULL,
    "FirstName" character varying(20) NOT NULL,
    "LastName" character varying(20) NOT NULL,
    "Email" character varying(200) NOT NULL,
    "Username" character varying(50) NOT NULL,
    "Password" bytea,
    "Role" user_roles NOT NULL,
    "VerifiedOn" timestamp with time zone,
    "ProfileImageUrl" text,
    "IsDisabled" boolean NOT NULL DEFAULT FALSE,
    "Settings" jsonb NOT NULL,
    CONSTRAINT "PK_UserAccounts" PRIMARY KEY ("Id")
);

CREATE TABLE "UserAccountVerifications" (
    "VerificationToken" text NOT NULL,
    "UserId" uuid NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_UserAccountVerifications" PRIMARY KEY ("VerificationToken"),
    CONSTRAINT "FK_UserAccountVerifications_UserAccounts_UserId" FOREIGN KEY ("UserId") REFERENCES "UserAccounts" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_UserAccount_CreatedOn" ON "UserAccounts" ("CreatedOn");

CREATE INDEX "IX_UserAccount_IsDeleted_ActiveOnly" ON "UserAccounts" ("IsDeleted") WHERE "IsDeleted" = false;

CREATE INDEX "IX_UserAccount_UpdatedOn" ON "UserAccounts" ("UpdatedOn");

CREATE UNIQUE INDEX "IX_UserAccounts_Email" ON "UserAccounts" ("Email");

CREATE UNIQUE INDEX "IX_UserAccounts_Username" ON "UserAccounts" ("Username");

CREATE INDEX "IX_UserAccounts_VectorText" ON "UserAccounts" USING GIN ("VectorText");

CREATE UNIQUE INDEX "IX_UserAccountVerifications_UserId" ON "UserAccountVerifications" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250924093753_Initial', '9.0.9');

COMMIT;

