using System.Windows.Controls;

namespace Restless.Panama.View
{
    /// <summary>
    /// Interaction logic for SubmissionTitleStatusToolTip.xaml
    /// </summary>
    public partial class SubmissionTitleStatusToolTip : Grid
    {
        public SubmissionTitleStatusToolTip()
        {
            InitializeComponent();
        }

        public static SubmissionTitleStatusToolTip Create(object dataContext)
        {
            return new SubmissionTitleStatusToolTip()
            {
                DataContext = dataContext
            };
        }
    }
}