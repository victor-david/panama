CREATE TABLE "config" (
  "id" TEXT NOT NULL, 
  "description" TEXT NOT NULL, 
  "type" TEXT NOT NULL, 
  "value" TEXT, 
  "edit" BOOLEAN DEFAULT 1, 
  PRIMARY KEY ("id")
);