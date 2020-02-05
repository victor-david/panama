CREATE TABLE "selfpublisher" (
  "id" INTEGER PRIMARY KEY NOT NULL,
  "name" TEXT NOT NULL,
  "url" TEXT,
  "notes" TEXT,
  "added" TIMESTAMP NOT NULL DEFAULT 0
);