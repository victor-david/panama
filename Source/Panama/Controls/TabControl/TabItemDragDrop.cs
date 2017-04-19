using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Restless.Tools.Utility;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Represents the pair of TabItem objects involved in a drag drop operation.
    /// </summary>
    public class TabItemDragDrop
    {
        /// <summary>
        /// Gets the source tab item.
        /// </summary>
        public TabItem Source
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the target tab item.
        /// </summary>
        public TabItem Target
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabItemDragDrop"/> class.
        /// </summary>
        /// <param name="source">The source tab item object.</param>
        /// <param name="target">The target tab item object.</param>
        public TabItemDragDrop(TabItem source, TabItem target)
        {
            Validations.ValidateNull(source, "TabItemDragDrop.Source");
            Validations.ValidateNull(target, "TabItemDragDrop.Target");
            Source = source;
            Target = target;
        }
    }
}
