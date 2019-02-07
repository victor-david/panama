CREATE TABLE "link" (
  "id" INTEGER PRIMARY KEY NOT NULL,
  "name" TEXT NOT NULL, 
  "url" TEXT NOT NULL, 
  "notes" TEXT, 
  "credentialid" INTEGER NOT NULL DEFAULT 0, 
  "added" TIMESTAMP NOT NULL
);
CREATE INDEX "link_credentialid" ON "link" ("credentialid");
