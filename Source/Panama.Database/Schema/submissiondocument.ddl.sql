CREATE TABLE "submissiondocument" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "submissionbatchid" INTEGER NOT NULL, 
  "title" VARCHAR(255) NOT NULL, 
  "doctype" INTEGER NOT NULL, 
  "docid" VARCHAR(255), 
  "updated" TIMESTAMP NOT NULL,
  "size" INTEGER NOT NULL DEFAULT 0
);