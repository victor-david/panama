CREATE TABLE "usernote" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "title" VARCHAR(128) NOT NULL, 
  "created" TIMESTAMP NOT NULL, 
  "note" TEXT 
);