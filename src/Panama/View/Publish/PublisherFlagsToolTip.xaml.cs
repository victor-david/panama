using System.Windows.Controls;

namespace Restless.Panama.View
{
    /// <summary>
    /// Interaction logic for PublisherFlagsToolTip.xaml
    /// </summary>
    public partial class PublisherFlagsToolTip : Grid
    {
        public PublisherFlagsToolTip()
        {
            InitializeComponent();
        }

        public static PublisherFlagsToolTip Create(object dataContext)
        {
            return new PublisherFlagsToolTip()
            {
                DataContext = dataContext
            };
        }
    }
}