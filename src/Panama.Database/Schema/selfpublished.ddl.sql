CREATE TABLE "selfpublished" (
  "id" INTEGER PRIMARY KEY NOT NULL,
  "titleid" INTEGER NOT NULL,
  "selfpublisherid" INTEGER NOT NULL DEFAULT 0,
  "added" TIMESTAMP NOT NULL,
  "published" TIMESTAMP,
  "url" TEXT,
  "notes" TEXT
);
CREATE INDEX "selfpublished_selfpublisherid" ON "selfpublished" ("selfpublisherid");
CREATE INDEX "selfpublished_titleid" ON "selfpublished" ("titleid");
