CREATE TABLE "published" (
  "id" INTEGER PRIMARY KEY NOT NULL,
  "titleid" INTEGER NOT NULL,
  "publisherid" INTEGER NOT NULL DEFAULT 0,
  "added" TIMESTAMP NOT NULL,
  "url" TEXT,
  "notes" TEXT
);
CREATE INDEX "published_publisherid" ON "published" ("publisherid");
CREATE INDEX "published_titleid" ON "published" ("titleid");
