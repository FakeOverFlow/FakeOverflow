START TRANSACTION;
CREATE TABLE "RefreshTokens" (
    "Id" text NOT NULL,
    "JTI" text NOT NULL,
    "UserId" uuid NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "ExpiresOn" timestamp with time zone NOT NULL DEFAULT (NOW() + INTERVAL '10 days'),
    "RevokedOn" timestamp with time zone,
    CONSTRAINT "PK_RefreshTokens" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_RefreshTokens_UserAccounts_UserId" FOREIGN KEY ("UserId") REFERENCES "UserAccounts" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_RefreshTokens_Id_UserId_ExpiresOn" ON "RefreshTokens" ("Id", "UserId", "ExpiresOn");

CREATE UNIQUE INDEX "IX_RefreshTokens_JTI_UserId" ON "RefreshTokens" ("JTI", "UserId");

CREATE UNIQUE INDEX "IX_RefreshTokens_UserId" ON "RefreshTokens" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250928100104_added_refresh_token', '9.0.9');

COMMIT;

