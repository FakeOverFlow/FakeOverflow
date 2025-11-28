START TRANSACTION;
ALTER TYPE public.content_type ADD VALUE 'analysis' BEFORE 'answers';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251127200044_add_analysis', '9.0.9');

COMMIT;

