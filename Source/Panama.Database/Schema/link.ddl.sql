CREATE TABLE "link" (
  "id" INTEGER PRIMARY KEY NOT NULL,
  "name" VARCHAR(160) NOT NULL, 
  "url" VARCHAR(255) NOT NULL, 
  "notes" VARCHAR(255), 
  "credentialid" INTEGER NOT NULL DEFAULT 0, 
  "added" TIMESTAMP NOT NULL
);
CREATE INDEX "link_credentialid" ON "link" ("credentialid");
