CREATE TABLE "submissionbatch" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "publisherid" INTEGER, 
  "fee" NUMERIC, 
  "award" NUMERIC, 
  "online" BOOLEAN NOT NULL DEFAULT '0', 
  "contest" BOOLEAN NOT NULL DEFAULT '0', 
  "locked" BOOLEAN NOT NULL DEFAULT '0', 
  "submitted" TIMESTAMP NOT NULL DEFAULT '0', 
  "response" TIMESTAMP, 
  "responsetype" INTEGER NOT NULL DEFAULT 0, 
  "notes" TEXT
);
