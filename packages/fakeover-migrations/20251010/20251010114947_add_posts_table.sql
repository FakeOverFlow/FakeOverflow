START TRANSACTION;
CREATE TYPE public.content_type AS ENUM ('answers', 'questions');

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

CREATE INDEX "IX_PostContent_CreatedBy" ON public."PostContent" ("CreatedBy");

CREATE UNIQUE INDEX "IX_PostContent_PostId_ContentType" ON public."PostContent" ("PostId", "ContentType") WHERE "ContentType" = 'questions';

CREATE INDEX "IX_PostContent_UpdatedBy" ON public."PostContent" ("UpdatedBy");

CREATE INDEX "IX_PostContent_VectorText" ON public."PostContent" USING GIN ("VectorText");

CREATE INDEX "IX_Posts_CreatedBy" ON public."Posts" ("CreatedBy");

CREATE INDEX "IX_Posts_UpdatedBy" ON public."Posts" ("UpdatedBy");

CREATE INDEX "IX_Posts_VectorText" ON public."Posts" USING GIN ("VectorText");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251010114947_add_posts_table', '9.0.9');

COMMIT;

