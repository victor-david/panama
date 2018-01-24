using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Interaction logic for ColorEdit.xaml
    /// </summary>
    public partial class ColorEdit : UserControl
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the selected color used for this control
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// Dependency property definition for the <see cref="SelectedColor"/> property
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
            (
                nameof(SelectedColor), typeof(Color), typeof(ColorEdit), new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );

        /// <summary>
        /// Gets or sets the title for this control
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Dependency property definition for the <see cref="Title"/> property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register
            (
                nameof(Title), typeof(string), typeof(ColorEdit), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets a value that indicates if <see cref="SelectedColor"/> represents a background color.
        /// </summary>
        public bool IsBackground
        {
            get { return (bool)GetValue(IsBackgroundProperty); }
            set { SetValue(IsBackgroundProperty, value); }
        }

        /// <summary>
        /// Dependency property definition for the <see cref="IsBackground"/> property
        /// </summary>
        public static readonly DependencyProperty IsBackgroundProperty = DependencyProperty.Register
            (
                nameof(IsBackground), typeof(bool), typeof(ColorEdit), new PropertyMetadata(true)
            );
        
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorEdit"/> class.
        /// </summary>
        public ColorEdit()
        {
            InitializeComponent();
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        #endregion

    }
}
