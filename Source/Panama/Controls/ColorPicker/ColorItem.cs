using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Represents a single color item.
    /// </summary>
    public class ColorItem
    {
        #region Public properties
        /// <summary>
        /// Gets or sets the color for this item.
        /// </summary>
        public Color Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of this item
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorItem"/> class.
        /// </summary>
        /// <param name="color">The color</param>
        /// <param name="name">The associated name</param>
        public ColorItem(Color color, string name)
        {
            Color = color;
            Name = name;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Determines if two instances are equal
        /// </summary>
        /// <param name="obj">The instance to compare to this instance.</param>
        /// <returns>true if color and name are the same; otherwise, false</returns>
        public override bool Equals(object obj)
        {
            var ci = obj as ColorItem;
            if (ci == null)  return false;
            return (ci.Color.Equals(Color) && ci.Name.Equals(Name));
        }

        /// <summary>
        /// Gets a hash code for this instance.
        /// </summary>
        /// <returns>A hash code created from the color and the name</returns>
        public override int GetHashCode()
        {
            return Color.GetHashCode() ^ Name.GetHashCode();
        }
        #endregion
    }
}
