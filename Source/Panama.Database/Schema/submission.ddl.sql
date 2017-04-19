CREATE TABLE "submission"( 
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "titleid" INTEGER NOT NULL, 
  "submissionbatchid" INTEGER NOT NULL, 
  "ordering" INTEGER DEFAULT 0, 
  "status" INTEGER NOT NULL DEFAULT 0, 
  "added" TIMESTAMP NOT NULL DEFAULT 0);
CREATE UNIQUE INDEX "submission_batchid_titleid" ON "submission" ("titleid" ,"submissionbatchid");