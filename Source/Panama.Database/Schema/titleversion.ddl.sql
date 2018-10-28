CREATE TABLE "titleversion" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "titleid" INTEGER NOT NULL DEFAULT 0, 
  "doctype" INTEGER NOT NULL DEFAULT 0, 
  "filename" TEXT NOT NULL, 
  "note" TEXT, 
  "updated" TIMESTAMP NOT NULL, 
  "size" INTEGER NOT NULL DEFAULT 0,
  "version" INTEGER NOT NULL DEFAULT 0,
  "revision" INTEGER NOT NULL DEFAULT 65,
  "langid" TEXT NOT NULL,
  "wordcount" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX "titleversion_langid" ON "titleversion" ("langid");
CREATE INDEX "titleversion_titleid" ON "titleversion" ("titleid");

