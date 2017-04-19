CREATE TABLE "submissionperiod" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "publisherid" INTEGER NOT NULL, 
  "start" TIMESTAMP NOT NULL, 
  "end" TIMESTAMP NOT NULL,
  "notes" TEXT
);