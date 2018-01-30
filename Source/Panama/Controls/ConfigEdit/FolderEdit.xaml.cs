using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.App.Panama.Resources;
using Restless.App.Panama.ViewModel;
using Restless.Tools.Search;
using System.Windows;
using System.Windows.Controls;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Interaction logic for FolderEdit.xaml
    /// </summary>
    public partial class FolderEdit : UserControl
    {
        #region Public properties
        /// <summary>
        /// Gets or sets the title of this control
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
                "Title", typeof(string), typeof(FolderEdit), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the folder.
        /// </summary>
        public string Folder
        {
            get { return (string)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        /// <summary>
        /// Dependency property definition for the <see cref="Folder"/> property.
        /// </summary>
        public static readonly DependencyProperty FolderProperty = DependencyProperty.Register
            (
                "Folder", typeof(string), typeof(FolderEdit), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );


        /// <summary>
        /// Gets or the sets the editing type for this control.
        /// </summary>
        public FolderEditType EditType
        {
            get { return (FolderEditType)GetValue(EditTypeProperty); }
            set { SetValue(EditTypeProperty, value); }
        }

        /// <summary>
        /// Dependency property definition for the <see cref="EditType"/> property.
        /// </summary>
        public static readonly DependencyProperty EditTypeProperty = DependencyProperty.Register
            (
                "EditType", typeof(FolderEditType), typeof(FolderEdit), new PropertyMetadata(FolderEditType.FileSystem)
            );

        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderEdit"/> class.
        /// </summary>
        public FolderEdit()
        {
            InitializeComponent();
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void ButtonChangeClick(object sender, RoutedEventArgs e)
        {
            switch (EditType)
            {
                case FolderEditType.FileSystem:
                    SelectFileSystemFolder();
                    break;
                case FolderEditType.Mapi:
                    SelectMapiFolder();
                    break;
            }
        }

        private void SelectFileSystemFolder()
        {
            using (var dialog = CommonDialogFactory.Create(Folder, "Select a directory", true))
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {

                    Folder = dialog.FileName;
                }
            }
        }

        private void SelectMapiFolder()
        {
            MessageSelectOptions ops = new MessageSelectOptions(MessageSelectMode.Folder, null);
            var w = WindowFactory.MessageSelect.Create(Strings.CaptionSelectMapiFolder, ops);
            w.ShowDialog();

            if (w.GetValue(WindowViewModel.ViewModelProperty) is MessageSelectWindowViewModel vm && vm.SelectedItems != null)
            {
                if (vm.SelectedItems[0] is WindowsSearchResult result)
                {
                    string url = result.Values[SysProps.System.ItemUrl].ToString();
                    // remove "mapi:" from string
                    Folder = url.Remove(0, 5);
                }
            }
        }
        #endregion
    }
}
