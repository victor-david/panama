using System.Windows.Controls;

namespace Restless.Panama.View
{
    /// <summary>
    /// Interaction logic for TitleFlagsToolTip.xaml
    /// </summary>
    public partial class TitleFlagsToolTip : Grid
    {
        public TitleFlagsToolTip()
        {
            InitializeComponent();
        }

        public static TitleFlagsToolTip Create(object dataContext)
        {
            return new TitleFlagsToolTip()
            {
                DataContext = dataContext
            };
        }
    }
}