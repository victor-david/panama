using System.Windows;
using System.Windows.Controls;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Interaction logic for YesNo.xaml
    /// </summary>
    public partial class YesNoEdit : UserControl
    {
        #region Public properties
        /// <summary>
        /// Gets or sets the group name for the control.
        /// </summary>
        public string GroupName
        {
            get => (string)GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        /// <summary>
        /// Dependency property definition for the <see cref="GroupName"/> property.
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register
            (
                "GroupName", typeof(string), typeof(YesNoEdit), new PropertyMetadata("Group")
            );

        /// <summary>
        /// Gets or sets the header text for the control.
        /// </summary>
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Dependency property definition for the <see cref="Header"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register
            (
                "Header", typeof(string), typeof(YesNoEdit), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets a value that indicates if the YES option is selected
        /// </summary>
        public bool IsYes
        {
            get => (bool)GetValue(IsYesProperty);
            set => SetValue(IsYesProperty, value);
        }

        /// <summary>
        /// Dependency property definition for the <see cref="IsYes"/> property.
        /// </summary>
        public static readonly DependencyProperty IsYesProperty = DependencyProperty.Register
            (
                "IsYes", typeof(bool), typeof(YesNoEdit), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );

        /// <summary>
        /// Gets a boolean value that indicates if the current value of the control is no.
        /// </summary>
        public bool IsNo
        {
            get => !IsYes;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="YesNoEdit"/> class.
        /// </summary>
        public YesNoEdit()
        {
            InitializeComponent();
        }
        #endregion
    }
}
