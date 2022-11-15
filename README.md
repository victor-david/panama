# Panama ∑en
A Windows desktop application for writers. Manages titles (articles, stories, etc.), publishers, submissions and more. 

## General Description
**Panama ∑en** is a desktop application for writers that enables you to keep track of various types of writing (articles, stories, etc), 
and where they've been sent for possible publication. Each title may have an unlimited number of versions associated with it.
Titles may be tagged in any number of ways, and the list of titles may be filtered by various criteria, including by tag.

## Publishers and Submissions
Information about publishers and submissions is central to the app. You can add publishers, create a submission,
add titles that are to be included as part of the submission, create documents associated with the submission, etc.
When responses to submissions come in, you can update the associated submission record, create records of publication
status (publication name, url, etc). With **Panama ∑en**, you can track everything from the creation of a piece of writing,
through its submission process until publication.

## Other Features
**Panama ∑en** includes many other features. You can create various notes and links to writing resources; search the text of files;
consolidate files to a single export directory; create a list of titles; view title and submission statistics, etc.

## Downloading
If you want to simply download and start using Panana, grab the zip file (binaries) from the latest release, unzip everything to the
directory of your choice, and run **Panama.exe**

By default the database file (which is created on first run) will be created here:

~~~
C:\Users\<user>\AppData\Roaming\RestlessAnimal\Panama
~~~

You can change the database location in settings, but keep in mind that it doesn't copy the previous data. You must copy manually
or (for a brand new setup) change the database location before you add any data. When you change the database
location, it takes effect once you close and reopen the app.

## Dependencies
**Panama ∑en** references various Nuget packages. They should download and install automatically when 
you build the solution.
