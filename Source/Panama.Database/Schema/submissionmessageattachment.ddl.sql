CREATE TABLE "submissionmessageattachment" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "messageid" INTEGER NOT NULL, 
  "display" TEXT NOT NULL, 
  "filename" TEXT NOT NULL, 
  "filesize" INTEGER NOT NULL, 
  "type" INTEGER NOT NULL DEFAULT 0 
);
CREATE INDEX "submissionmessageattachment_messageid" ON "submissionmessageattachment" ("messageid");