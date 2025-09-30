START TRANSACTION;
ALTER TABLE public."UserAccounts" DROP COLUMN "VectorText";

ALTER TABLE public."UserAccounts" DROP COLUMN "FirstName";

ALTER TABLE public."UserAccounts" DROP COLUMN "LastName";

ALTER TABLE public."UserAccounts" ADD "VectorText" tsvector GENERATED ALWAYS AS (to_tsvector('english', "Email" || ' ' || "Username")) STORED NOT NULL;

CREATE INDEX "IX_UserAccounts_VectorText" ON public."UserAccounts" USING GIN ("VectorText");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250930113015_remove_first-last_names', '9.0.9');

COMMIT;

