using System;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Simple adapter class to add an item to <see cref="SearchTable"/>
    /// </summary>
    public class SearchRowItem
    {
        public string Type { get; set; }
        public string File { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Company { get; set; }
        public int Size { get; set; }
        public bool IsVersion { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}