﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Restless.App.Panama.Database.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Create {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Create() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Restless.App.Panama.Database.Resources.Create", typeof(Create).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;alert&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;title&quot; TEXT NOT NULL, 
        ///  &quot;date&quot; TIMESTAMP NOT NULL,
        ///  &quot;enabled&quot; BOOLEAN DEFAULT 0
        ///);.
        /// </summary>
        internal static string Alert {
            get {
                return ResourceManager.GetString("Alert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;author&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;name&quot; TEXT NOT NULL,
        ///  &quot;isdefault&quot; BOOLEAN DEFAULT 0
        ///).
        /// </summary>
        internal static string Author {
            get {
                return ResourceManager.GetString("Author", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;color&quot; (
        ///  &quot;id&quot; TEXT NOT NULL,
        ///  &quot;foreground&quot; TEXT NOT NULL,
        ///  &quot;background&quot; TEXT NOT NULL,
        ///  PRIMARY KEY (&quot;id&quot;)
        ///);.
        /// </summary>
        internal static string Color {
            get {
                return ResourceManager.GetString("Color", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;config&quot; (
        ///  &quot;id&quot; TEXT NOT NULL, 
        ///  &quot;value&quot; TEXT, 
        ///  PRIMARY KEY (&quot;id&quot;)
        ///);.
        /// </summary>
        internal static string Config {
            get {
                return ResourceManager.GetString("Config", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;credential&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;name&quot; TEXT NOT NULL, 
        ///  &quot;loginid&quot; TEXT NOT NULL, 
        ///  &quot;password&quot; TEXT NOT NULL 
        ///);.
        /// </summary>
        internal static string Credential {
            get {
                return ResourceManager.GetString("Credential", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;documenttype&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;name&quot; TEXT NOT NULL, 
        ///  &quot;extensions&quot; TEXT, 
        ///  &quot;ordering&quot; INTEGER NOT NULL,
        ///  &quot;supported&quot; BOOLEAN NOT NULL DEFAULT 1 
        ///);.
        /// </summary>
        internal static string DocumentType {
            get {
                return ResourceManager.GetString("DocumentType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;language&quot; (
        ///  &quot;id&quot; TEXT NOT NULL, 
        ///  &quot;name&quot; TEXT NOT NULL, 
        ///  PRIMARY KEY (&quot;id&quot;)
        ///);.
        /// </summary>
        internal static string Language {
            get {
                return ResourceManager.GetString("Language", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;link&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL,
        ///  &quot;name&quot; TEXT NOT NULL, 
        ///  &quot;url&quot; TEXT NOT NULL, 
        ///  &quot;notes&quot; TEXT, 
        ///  &quot;credentialid&quot; INTEGER NOT NULL DEFAULT 0, 
        ///  &quot;added&quot; TIMESTAMP NOT NULL
        ///);
        ///CREATE INDEX &quot;link_credentialid&quot; ON &quot;link&quot; (&quot;credentialid&quot;);.
        /// </summary>
        internal static string Link {
            get {
                return ResourceManager.GetString("Link", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;published&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL,
        ///  &quot;titleid&quot; INTEGER NOT NULL,
        ///  &quot;publisherid&quot; INTEGER NOT NULL DEFAULT 0,
        ///  &quot;added&quot; TIMESTAMP NOT NULL,
        ///  &quot;published&quot; TIMESTAMP,
        ///  &quot;url&quot; TEXT,
        ///  &quot;notes&quot; TEXT
        ///);
        ///CREATE INDEX &quot;published_publisherid&quot; ON &quot;published&quot; (&quot;publisherid&quot;);
        ///CREATE INDEX &quot;published_titleid&quot; ON &quot;published&quot; (&quot;titleid&quot;);.
        /// </summary>
        internal static string Published {
            get {
                return ResourceManager.GetString("Published", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;publisher&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL,
        ///  &quot;name&quot; TEXT NOT NULL,
        ///  &quot;address1&quot; TEXT,
        ///  &quot;address2&quot; TEXT,
        ///  &quot;address3&quot; TEXT,
        ///  &quot;city&quot; TEXT,
        ///  &quot;state&quot; TEXT,
        ///  &quot;zip&quot; TEXT,
        ///  &quot;url&quot; TEXT,
        ///  &quot;email&quot; TEXT,
        ///  &quot;options&quot; INTEGER NOT NULL DEFAULT 0,
        ///  &quot;credentialid&quot; INTEGER NOT NULL DEFAULT 0,
        ///  &quot;notes&quot; TEXT,
        ///  &quot;added&quot; TIMESTAMP NOT NULL DEFAULT 0,
        ///  &quot;exclusive&quot; BOOLEAN NOT NULL DEFAULT 0,
        ///  &quot;paying&quot; BOOLEAN NOT NULL DEFAULT 0,
        ///  &quot;followup&quot; BOOLEAN NOT NULL DEFAULT 0,
        ///  &quot;goner&quot; B [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Publisher {
            get {
                return ResourceManager.GetString("Publisher", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;response&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;name&quot; TEXT, 
        ///  &quot;description&quot; TEXT 
        ///);.
        /// </summary>
        internal static string Response {
            get {
                return ResourceManager.GetString("Response", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;schema&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;version&quot; INTEGER NOT NULL DEFAULT 0, 
        ///  &quot;ts&quot; TIMESTAMP NOT NULL DEFAULT 0 
        ///);.
        /// </summary>
        internal static string Schema {
            get {
                return ResourceManager.GetString("Schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;selfpublished&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL,
        ///  &quot;titleid&quot; INTEGER NOT NULL,
        ///  &quot;selfpublisherid&quot; INTEGER NOT NULL DEFAULT 0,
        ///  &quot;added&quot; TIMESTAMP NOT NULL,
        ///  &quot;published&quot; TIMESTAMP,
        ///  &quot;url&quot; TEXT,
        ///  &quot;notes&quot; TEXT
        ///);
        ///CREATE INDEX &quot;selfpublished_selfpublisherid&quot; ON &quot;selfpublished&quot; (&quot;selfpublisherid&quot;);
        ///CREATE INDEX &quot;selfpublished_titleid&quot; ON &quot;selfpublished&quot; (&quot;titleid&quot;);.
        /// </summary>
        internal static string SelfPublished {
            get {
                return ResourceManager.GetString("SelfPublished", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;selfpublisher&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL,
        ///  &quot;name&quot; TEXT NOT NULL,
        ///  &quot;url&quot; TEXT,
        ///  &quot;notes&quot; TEXT,
        ///  &quot;added&quot; TIMESTAMP NOT NULL DEFAULT 0
        ///);.
        /// </summary>
        internal static string SelfPublisher {
            get {
                return ResourceManager.GetString("SelfPublisher", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;submission&quot;( 
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;titleid&quot; INTEGER NOT NULL, 
        ///  &quot;submissionbatchid&quot; INTEGER NOT NULL, 
        ///  &quot;ordering&quot; INTEGER DEFAULT 0, 
        ///  &quot;status&quot; INTEGER NOT NULL DEFAULT 0, 
        ///  &quot;added&quot; TIMESTAMP NOT NULL DEFAULT 0);
        ///CREATE UNIQUE INDEX &quot;submission_batchid_titleid&quot; ON &quot;submission&quot; (&quot;titleid&quot; ,&quot;submissionbatchid&quot;);.
        /// </summary>
        internal static string Submission {
            get {
                return ResourceManager.GetString("Submission", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;submissionbatch&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;publisherid&quot; INTEGER, 
        ///  &quot;fee&quot; NUMERIC, 
        ///  &quot;award&quot; NUMERIC, 
        ///  &quot;online&quot; BOOLEAN NOT NULL DEFAULT &apos;0&apos;, 
        ///  &quot;contest&quot; BOOLEAN NOT NULL DEFAULT &apos;0&apos;, 
        ///  &quot;locked&quot; BOOLEAN NOT NULL DEFAULT &apos;0&apos;, 
        ///  &quot;submitted&quot; TIMESTAMP NOT NULL DEFAULT &apos;0&apos;, 
        ///  &quot;response&quot; TIMESTAMP, 
        ///  &quot;responsetype&quot; INTEGER NOT NULL DEFAULT 0, 
        ///  &quot;notes&quot; TEXT
        ///);.
        /// </summary>
        internal static string SubmissionBatch {
            get {
                return ResourceManager.GetString("SubmissionBatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;submissiondocument&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;submissionbatchid&quot; INTEGER NOT NULL, 
        ///  &quot;title&quot; TEXT NOT NULL, 
        ///  &quot;doctype&quot; INTEGER NOT NULL, 
        ///  &quot;docid&quot; TEXT, 
        ///  &quot;updated&quot; TIMESTAMP NOT NULL,
        ///  &quot;size&quot; INTEGER NOT NULL DEFAULT 0
        ///);.
        /// </summary>
        internal static string SubmissionDocument {
            get {
                return ResourceManager.GetString("SubmissionDocument", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;submissionmessage&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;entryid&quot; TEXT NOT NULL, 
        ///  &quot;submissionbatchid&quot; INTEGER NOT NULL, 
        ///  &quot;outgoing&quot; BOOLEAN DEFAULT &apos;0&apos;, 
        ///  &quot;sendername&quot; TEXT NOT NULL, 
        ///  &quot;senderemail&quot; TEXT NOT NULL, 
        ///  &quot;recipientname&quot; TEXT NOT NULL, 
        ///  &quot;recipientemail&quot; TEXT NOT NULL, 
        ///  &quot;subject&quot; TEXT, 
        ///  &quot;display&quot; TEXT, 
        ///  &quot;sent&quot; TIMESTAMP NOT NULL, 
        ///  &quot;received&quot; TIMESTAMP NOT NULL, 
        ///  &quot;bodyformat&quot; INTEGER NOT NULL, 
        ///  &quot;body&quot; TEXT 
        ///);.
        /// </summary>
        internal static string SubmissionMessage {
            get {
                return ResourceManager.GetString("SubmissionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;submissionmessageattachment&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;messageid&quot; INTEGER NOT NULL, 
        ///  &quot;display&quot; TEXT NOT NULL, 
        ///  &quot;filename&quot; TEXT NOT NULL, 
        ///  &quot;filesize&quot; INTEGER NOT NULL, 
        ///  &quot;type&quot; INTEGER NOT NULL DEFAULT 0 
        ///);
        ///CREATE INDEX &quot;submissionmessageattachment_messageid&quot; ON &quot;submissionmessageattachment&quot; (&quot;messageid&quot;);.
        /// </summary>
        internal static string SubmissionMessageAttachment {
            get {
                return ResourceManager.GetString("SubmissionMessageAttachment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;submissionperiod&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;publisherid&quot; INTEGER NOT NULL, 
        ///  &quot;start&quot; TIMESTAMP NOT NULL, 
        ///  &quot;end&quot; TIMESTAMP NOT NULL,
        ///  &quot;notes&quot; TEXT
        ///);.
        /// </summary>
        internal static string SubmissionPeriod {
            get {
                return ResourceManager.GetString("SubmissionPeriod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;tag&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;tag&quot; TEXT NOT NULL, 
        ///  &quot;description&quot; TEXT NOT NULL, 
        ///  &quot;usagecount&quot; INTEGER NOT NULL DEFAULT 0
        ///);.
        /// </summary>
        internal static string Tag {
            get {
                return ResourceManager.GetString("Tag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;title&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;title&quot; TEXT NOT NULL, 
        ///  &quot;created&quot; TIMESTAMP NOT NULL, 
        ///  &quot;written&quot; TIMESTAMP NOT NULL, 
        ///  &quot;authorid&quot; INTEGER NOT NULL DEFAULT 1, 
        ///  &quot;ready&quot; BOOLEAN NOT NULL DEFAULT 0,
        ///  &quot;qflag&quot; BOOLEAN NOT NULL DEFAULT 0,
        ///  &quot;notes&quot; TEXT
        ///);.
        /// </summary>
        internal static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;titletag&quot; (
        ///  &quot;titleid&quot; INTEGER NOT NULL, 
        ///  &quot;tagid&quot; INTEGER NOT NULL, 
        ///  PRIMARY KEY (&quot;titleid&quot;, &quot;tagid&quot;)
        ///);.
        /// </summary>
        internal static string TitleTag {
            get {
                return ResourceManager.GetString("TitleTag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;titleversion&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;titleid&quot; INTEGER NOT NULL DEFAULT 0, 
        ///  &quot;doctype&quot; INTEGER NOT NULL DEFAULT 0, 
        ///  &quot;filename&quot; TEXT NOT NULL, 
        ///  &quot;note&quot; TEXT, 
        ///  &quot;updated&quot; TIMESTAMP NOT NULL, 
        ///  &quot;size&quot; INTEGER NOT NULL DEFAULT 0,
        ///  &quot;version&quot; INTEGER NOT NULL DEFAULT 0,
        ///  &quot;revision&quot; INTEGER NOT NULL DEFAULT 65,
        ///  &quot;langid&quot; TEXT NOT NULL,
        ///  &quot;wordcount&quot; INTEGER NOT NULL DEFAULT 0
        ///);
        ///CREATE INDEX &quot;titleversion_langid&quot; ON &quot;titleversion&quot; (&quot;langid&quot;);
        ///CREATE INDEX &quot;ti [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TitleVersion {
            get {
                return ResourceManager.GetString("TitleVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE TABLE &quot;usernote&quot; (
        ///  &quot;id&quot; INTEGER PRIMARY KEY NOT NULL, 
        ///  &quot;title&quot; TEXT NOT NULL, 
        ///  &quot;created&quot; TIMESTAMP NOT NULL, 
        ///  &quot;note&quot; TEXT 
        ///);.
        /// </summary>
        internal static string UserNote {
            get {
                return ResourceManager.GetString("UserNote", resourceCulture);
            }
        }
    }
}
