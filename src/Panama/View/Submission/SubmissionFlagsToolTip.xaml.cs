using System.Windows.Controls;

namespace Restless.Panama.View
{
    /// <summary>
    /// Interaction logic for SubmissionFlagsToolTip.xaml
    /// </summary>
    public partial class SubmissionFlagsToolTip : Grid
    {
        public SubmissionFlagsToolTip()
        {
            InitializeComponent();
        }

        public static SubmissionFlagsToolTip Create(object dataContext)
        {
            return new SubmissionFlagsToolTip()
            {
                DataContext = dataContext
            };
        }
    }
}