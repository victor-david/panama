CREATE TABLE "tag" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "tag" VARCHAR(16) NOT NULL, 
  "description" VARCHAR(80) NOT NULL, 
  "usagecount" INTEGER NOT NULL DEFAULT 0 
);