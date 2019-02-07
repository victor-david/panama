CREATE TABLE "publisher" (
  "id" INTEGER PRIMARY KEY NOT NULL,
  "name" TEXT NOT NULL,
  "address1" TEXT,
  "address2" TEXT,
  "address3" TEXT,
  "city" TEXT,
  "state" TEXT,
  "zip" TEXT,
  "url" TEXT,
  "email" TEXT,
  "options" INTEGER NOT NULL DEFAULT 0,
  "credentialid" INTEGER NOT NULL DEFAULT 0,
  "notes" TEXT,
  "added" TIMESTAMP NOT NULL DEFAULT 0,
  "exclusive" BOOLEAN NOT NULL DEFAULT 0,
  "paying" BOOLEAN NOT NULL DEFAULT 0,
  "followup" BOOLEAN NOT NULL DEFAULT 0,
  "goner" BOOLEAN NOT NULL DEFAULT 0
);

CREATE INDEX "publisher_credentialid" ON "publisher" ("credentialid");