using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Restless.App.Panama.Controls
{
    public class ColorItem
    {
        public Color Color
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public ColorItem(Color color, string name)
        {
            Color = color;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var ci = obj as ColorItem;
            if (ci == null)  return false;
            return (ci.Color.Equals(Color) && ci.Name.Equals(Name));
        }

        public override int GetHashCode()
        {
            return this.Color.GetHashCode() ^ Name.GetHashCode();
        }
    }
}
