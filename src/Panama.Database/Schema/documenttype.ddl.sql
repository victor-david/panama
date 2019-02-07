CREATE TABLE "documenttype" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "name" TEXT NOT NULL, 
  "extensions" TEXT, 
  "ordering" INTEGER NOT NULL,
  "supported" BOOLEAN NOT NULL DEFAULT 1 
);