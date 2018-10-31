CREATE TABLE "title" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "title" TEXT NOT NULL, 
  "created" TIMESTAMP NOT NULL, 
  "written" TIMESTAMP NOT NULL, 
  "authorid" INTEGER NOT NULL DEFAULT 1, 
  "ready" BOOLEAN NOT NULL DEFAULT 0,
  "qflag" BOOLEAN NOT NULL DEFAULT 0,
  "notes" TEXT
);