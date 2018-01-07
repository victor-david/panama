CREATE TABLE "submissiondocument" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "submissionbatchid" INTEGER NOT NULL, 
  "title" TEXT NOT NULL, 
  "doctype" INTEGER NOT NULL, 
  "docid" TEXT, 
  "updated" TIMESTAMP NOT NULL,
  "size" INTEGER NOT NULL DEFAULT 0
);