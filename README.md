# Panama ∑en
A Windows desktop application for writers. Manages titles (articles, stories, etc.), publishers, submissions and more. 

## General Description
**Panama ∑en** is a desktop application for writers that enables you to keep track of various types of writing (articles, stories, etc), 
and where they've been sent for possible publication. Each title may have an unlimited number of versions associated with it. Titles may be tagged in any number of ways, 
and the list of titles may be filtered by various criteria, including by tag. The database used by **Panama ∑en** is SqlLite.

## Publishers and Submissions
Information about publishers and submissions is central to the app. You can add publishers, create a submission, add titles that are to be included as part of the submission, 
create documents associated with the submission, etc. When responses to submissions come in, you can update the associated submission record, 
create records of publication status (publication name, url, etc). With **Panama ∑en**, you can track everything from the creation of a piece of writing,
through its submission process until publication.

## Other Features
**Panama ∑en** includes many other features. You can create various notes and links to writing resources; keep track of login info for various submission portals; 
search the text of files; consolidate files to a single export directory; create a list of titles; view title and submission statistics, etc.

## Dependencies
The solution file for **Panama ∑en** references the **Restless Tools** project. To build, you'll need to use:

- [Restless Tools Library](https://github.com/victor-david/restless-tools)

## How to Build
- Download this project
- Download the [Restless Tools Library](https://github.com/victor-david/restless-tools)

Download both projects to the same parent directory, each in its own folder. Name the folders as shown:

|--Panama

|--Restless.Tools.Library

1. Open Restless.Tools.Library/Source/Restless.Tools.Library.sln in Visual Studio
2. Build
3. Open Panama/Source/Panama.sln in Visual Studio
4. Build

**NOTE:** Although Panama.sln uses the **Restless Tools** project, it's neccessary to build the library
first in order to allow the packages for the library to be downloaded. If you build Panama.sln first,
the packages will be downloaded, but they'll be relative to the Panama.sln and the library project references 
won't be correct. Once you've build the **Restless Tools** project, you can skip steps 1 and 2 in future builds
of **Panama ∑en**.

## .Doc to .Docx Conversion
The [Restless Tools Library](https://github.com/victor-david/restless-tools) dependency project contains a sub-project that enables batch conversion
of .doc files to .docx. This .dll project requires a reference to Microsoft.Office.Interop.Word. If you don't have MS Office installed, and/or want to build 
a version of **Panama ∑en** without this support, do the following:

- Remove the Restless.Tools.Library.OfficeAutomation from the application solution.
- On the application's property page, remove the conditional compilation symbol **DOCX**.
