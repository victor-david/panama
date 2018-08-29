CREATE TABLE "title" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "title" TEXT NOT NULL, 
  "written" TIMESTAMP NOT NULL, 
  "authorid" INTEGER NOT NULL DEFAULT 1, 
  "ready" BOOLEAN DEFAULT 0,
  "notes" TEXT
);