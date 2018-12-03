# Panama ∑en
A Windows desktop application for writers. Manages titles (articles, stories, etc.), publishers, submissions and more. 

## General Description
**Panama ∑en** is a desktop application for writers that enables you to keep track of various types of writing (articles, stories, etc), 
and where they've been sent for possible publication. Each title may have an unlimited number of versions associated with it.
Titles may be tagged in any number of ways, and the list of titles may be filtered by various criteria, including by tag.
The database used by **Panama ∑en** is SqlLite.

## Publishers and Submissions
Information about publishers and submissions is central to the app. You can add publishers, create a submission,
add titles that are to be included as part of the submission, create documents associated with the submission, etc.
When responses to submissions come in, you can update the associated submission record, create records of publication
status (publication name, url, etc). With **Panama ∑en**, you can track everything from the creation of a piece of writing,
through its submission process until publication.

## Other Features
**Panama ∑en** includes many other features. You can create various notes and links to writing resources; keep track of login info 
for various submission portals; search the text of files; consolidate files to a single export directory; create a list of titles;
view title and submission statistics, etc.

## Dependencies
**Panama ∑en** references various Nuget packages. They should download and install automatically when 
you build the solution.

## .Doc to .Docx Conversion
One of the packages that is referenced is 
~~~xml
<package id="Microsoft.Office.Interop.Word" version="15.0.4797.1003" targetFramework="net452" />
~~~

and is used for batch .doc to .docx file conversion. For historical reasons, the ability to convert from .doc to .docx
is controlled by the conditional compilation symbol **DOCX**. If you'd rather not have this conversion feature, you can remove
the symbol definition on the application's property page.