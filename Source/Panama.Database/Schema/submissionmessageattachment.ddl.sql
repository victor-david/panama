CREATE TABLE "submissionmessageattachment" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "messageid" INTEGER NOT NULL, 
  "display" VARCHAR(255) NOT NULL, 
  "filename" VARCHAR(255) NOT NULL, 
  "filesize" INTEGER NOT NULL, 
  "type" INTEGER NOT NULL DEFAULT 0 
);
CREATE INDEX "submissionmessageattachment_messageid" ON "submissionmessageattachment" ("messageid");