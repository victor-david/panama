CREATE TABLE "tag" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "tag" TEXT NOT NULL, 
  "description" TEXT NOT NULL, 
  "usagecount" INTEGER NOT NULL DEFAULT 0
);