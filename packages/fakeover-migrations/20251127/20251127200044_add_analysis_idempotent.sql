CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE TYPE public.user_roles AS ENUM ('admin', 'moderator', 'user');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE TABLE public."UserAccounts" (
        "Id" uuid NOT NULL,
        "CreatedBy" uuid NOT NULL,
        "CreatedOn" timestamp with time zone NOT NULL,
        "UpdatedBy" uuid NOT NULL,
        "UpdatedOn" timestamp with time zone NOT NULL,
        "IsDeleted" boolean NOT NULL DEFAULT FALSE,
        "VectorText" tsvector GENERATED ALWAYS AS (to_tsvector('english', "Email" || ' ' || "Username")) STORED NOT NULL,
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE TABLE public."RefreshTokens" (
        "Id" text NOT NULL,
        "JTI" text NOT NULL,
        "UserId" uuid NOT NULL,
        "CreatedOn" timestamp with time zone NOT NULL,
        "ExpiresOn" timestamp with time zone NOT NULL DEFAULT (NOW() + INTERVAL '10 days'),
        "RevokedOn" timestamp with time zone,
        CONSTRAINT "PK_RefreshTokens" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_RefreshTokens_UserAccounts_UserId" FOREIGN KEY ("UserId") REFERENCES public."UserAccounts" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE TABLE public."UserAccountVerifications" (
        "VerificationToken" text NOT NULL,
        "UserId" uuid NOT NULL,
        "CreatedOn" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_UserAccountVerifications" PRIMARY KEY ("VerificationToken"),
        CONSTRAINT "FK_UserAccountVerifications_UserAccounts_UserId" FOREIGN KEY ("UserId") REFERENCES public."UserAccounts" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE INDEX "IX_RefreshTokens_Id_UserId_ExpiresOn" ON public."RefreshTokens" ("Id", "UserId", "ExpiresOn");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE UNIQUE INDEX "IX_RefreshTokens_JTI_UserId" ON public."RefreshTokens" ("JTI", "UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE INDEX "IX_RefreshTokens_UserId" ON public."RefreshTokens" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE INDEX "IX_UserAccount_CreatedOn" ON public."UserAccounts" ("CreatedOn");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE INDEX "IX_UserAccount_IsDeleted_ActiveOnly" ON public."UserAccounts" ("IsDeleted") WHERE "IsDeleted" = false;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE INDEX "IX_UserAccount_UpdatedOn" ON public."UserAccounts" ("UpdatedOn");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE UNIQUE INDEX "IX_UserAccounts_Email" ON public."UserAccounts" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE UNIQUE INDEX "IX_UserAccounts_Username" ON public."UserAccounts" ("Username");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE INDEX "IX_UserAccounts_VectorText" ON public."UserAccounts" USING GIN ("VectorText");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    CREATE UNIQUE INDEX "IX_UserAccountVerifications_UserId" ON public."UserAccountVerifications" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930114253_Initial') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250930114253_Initial', '9.0.9');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010114947_add_posts_table') THEN
    CREATE TYPE public.content_type AS ENUM ('answers', 'questions');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010114947_add_posts_table') THEN
    CREATE TABLE public."Posts" (
        "Id" text NOT NULL,
        "CreatedBy" uuid NOT NULL,
        "CreatedOn" timestamp with time zone NOT NULL,
        "UpdatedBy" uuid NOT NULL,
        "UpdatedOn" timestamp with time zone NOT NULL,
        "Title" text NOT NULL,
        "Views" bigint NOT NULL,
        "Votes" bigint NOT NULL,
        "VectorText" tsvector GENERATED ALWAYS AS (to_tsvector('english', "Title")) STORED NOT NULL,
        CONSTRAINT "PK_Posts" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Posts_UserAccounts_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."UserAccounts" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Posts_UserAccounts_UpdatedBy" FOREIGN KEY ("UpdatedBy") REFERENCES public."UserAccounts" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010114947_add_posts_table') THEN
    CREATE TABLE public."PostContent" (
        "Id" uuid NOT NULL,
        "CreatedBy" uuid NOT NULL,
        "CreatedOn" timestamp with time zone NOT NULL,
        "UpdatedBy" uuid NOT NULL,
        "UpdatedOn" timestamp with time zone NOT NULL,
        "Content" text NOT NULL,
        "ContentType" content_type NOT NULL,
        "PostId" text NOT NULL,
        "VectorText" tsvector GENERATED ALWAYS AS (to_tsvector('english', "Content")) STORED NOT NULL,
        CONSTRAINT "PK_PostContent" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_PostContent_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES public."Posts" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_PostContent_UserAccounts_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."UserAccounts" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_PostContent_UserAccounts_UpdatedBy" FOREIGN KEY ("UpdatedBy") REFERENCES public."UserAccounts" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010114947_add_posts_table') THEN
    CREATE INDEX "IX_PostContent_CreatedBy" ON public."PostContent" ("CreatedBy");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010114947_add_posts_table') THEN
    CREATE UNIQUE INDEX "IX_PostContent_PostId_ContentType" ON public."PostContent" ("PostId", "ContentType") WHERE "ContentType" = 'questions';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010114947_add_posts_table') THEN
    CREATE INDEX "IX_PostContent_UpdatedBy" ON public."PostContent" ("UpdatedBy");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010114947_add_posts_table') THEN
    CREATE INDEX "IX_PostContent_VectorText" ON public."PostContent" USING GIN ("VectorText");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010114947_add_posts_table') THEN
    CREATE INDEX "IX_Posts_CreatedBy" ON public."Posts" ("CreatedBy");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010114947_add_posts_table') THEN
    CREATE INDEX "IX_Posts_UpdatedBy" ON public."Posts" ("UpdatedBy");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010114947_add_posts_table') THEN
    CREATE INDEX "IX_Posts_VectorText" ON public."Posts" USING GIN ("VectorText");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010114947_add_posts_table') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251010114947_add_posts_table', '9.0.9');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    DROP INDEX public."IX_PostContent_PostId_ContentType";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    ALTER TABLE public."Posts" DROP COLUMN "Votes";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    ALTER TABLE public."PostContent" ADD "PostsId" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    ALTER TABLE public."PostContent" ADD "Votes" bigint NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    CREATE TABLE public."Tag" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "Value" character varying(50) NOT NULL,
        "Description" character varying(200) NOT NULL DEFAULT '',
        "Color" character varying(20) NOT NULL,
        "VectorText" tsvector GENERATED ALWAYS AS (to_tsvector('english', "Value" || ' ' || "Description")) STORED NOT NULL,
        CONSTRAINT "PK_Tag" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    CREATE TABLE public."Votes" (
        "UserId" uuid NOT NULL,
        "ContentId" uuid NOT NULL,
        "UpVote" boolean NOT NULL,
        CONSTRAINT "PK_Votes" PRIMARY KEY ("UserId", "ContentId"),
        CONSTRAINT "FK_Votes_PostContent_ContentId" FOREIGN KEY ("ContentId") REFERENCES public."PostContent" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Votes_UserAccounts_UserId" FOREIGN KEY ("UserId") REFERENCES public."UserAccounts" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    CREATE TABLE public."PostTags" (
        "TagId" integer NOT NULL,
        "PostId" text NOT NULL,
        CONSTRAINT "PK_PostTags" PRIMARY KEY ("PostId", "TagId"),
        CONSTRAINT "FK_PostTags_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES public."Posts" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_PostTags_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES public."Tag" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    CREATE UNIQUE INDEX "IX_PostContent_PostId_ContentType" ON public."PostContent" ("PostId", "ContentType") WHERE "ContentType" = 'questions';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    CREATE INDEX "IX_PostContent_PostsId" ON public."PostContent" ("PostsId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    CREATE INDEX "IX_PostTags_TagId" ON public."PostTags" ("TagId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    CREATE UNIQUE INDEX "IX_Tag_Value" ON public."Tag" ("Value");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    CREATE INDEX "IX_Tag_VectorText" ON public."Tag" USING GIN ("VectorText");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    CREATE INDEX "IX_Votes_ContentId" ON public."Votes" ("ContentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    ALTER TABLE public."PostContent" ADD CONSTRAINT "FK_PostContent_Posts_PostsId" FOREIGN KEY ("PostsId") REFERENCES public."Posts" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251101102351_add_post_votes_tags') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251101102351_add_post_votes_tags', '9.0.9');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127200044_add_analysis') THEN
    ALTER TYPE public.content_type ADD VALUE 'analysis' BEFORE 'answers';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127200044_add_analysis') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251127200044_add_analysis', '9.0.9');
    END IF;
END $EF$;
COMMIT;

