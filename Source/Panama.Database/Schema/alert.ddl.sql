CREATE TABLE "alert" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "title" TEXT NOT NULL, 
  "date" TIMESTAMP NOT NULL,
  "enabled" BOOLEAN DEFAULT 0
);