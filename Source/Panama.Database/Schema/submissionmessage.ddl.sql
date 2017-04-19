CREATE TABLE "submissionmessage" (
  "id" INTEGER PRIMARY KEY NOT NULL, 
  "entryid" TEXT NOT NULL, 
  "submissionbatchid" INTEGER NOT NULL, 
  "outgoing" BOOLEAN DEFAULT '0', 
  "sendername" TEXT NOT NULL, 
  "senderemail" TEXT NOT NULL, 
  "recipientname" TEXT NOT NULL, 
  "recipientemail" TEXT NOT NULL, 
  "subject" TEXT, 
  "display" TEXT, 
  "sent" TIMESTAMP NOT NULL, 
  "received" TIMESTAMP NOT NULL, 
  "bodyformat" INTEGER NOT NULL, 
  "body" TEXT 
);