using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SystemColors = System.Windows.Media.Colors;

namespace Restless.App.Panama.Configuration
{
    /// <summary>
    /// Represents a single configuration color.
    /// </summary>
    public class ConfigColor : BindableBase
    {
        #region Private
        private ColorTable colorTable;
        private DataRow colorRow;
        private string id;
        private Color defaultColor;
        private bool defaultEnabled;
        #endregion

        /************************************************************************/

        #region Public Properties
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public Color Color
        {
            get => GetColor();
            set => SetColor(value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="Color"/> is enabled for use.
        /// </summary>
        public bool IsEnabled
        {
            get => GetIsEnabled();
            set => SetIsEnabled(value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigColor"/> class.
        /// </summary>
        /// <param name="id">The id associated with this color.</param>
        /// <param name="defaultColor">The default value associated with this color.</param>
        public ConfigColor(string id, Color defaultColor, bool defaultEnabled)
        {
            this.id = id ?? throw new ArgumentNullException();
            this.defaultColor = defaultColor;
            this.defaultEnabled = defaultEnabled;
            colorTable = DatabaseController.Instance.GetTable<ColorTable>();
            colorRow = colorTable.GetConfigurationRow(id, defaultColor, defaultEnabled);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Resets this configuration color to its default value.
        /// </summary>
        public void ResetToDefault()
        {
            Color = defaultColor;
            IsEnabled = defaultEnabled;
        }

        /// <summary>
        /// Get the brush according to the specified Color.
        /// </summary>
        /// <returns>The brush, or null if <see cref="HasValue"/> is false.</returns>
        public SolidColorBrush GetBrush()
        {
            if (IsEnabled)
            {
                return new SolidColorBrush(Color);
            }
            return null;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private Color GetColor()
        {
            string value = colorRow[ColorTable.Defs.Columns.Value].ToString();
            if (String.IsNullOrEmpty(value))
            {
                return SystemColors.Transparent;
            }
            try
            {
                return (Color)ColorConverter.ConvertFromString(value);
            }
            catch
            {
                return SystemColors.Transparent;
            }
        }

        private void SetColor(Color value)
        {
            if (value != GetColor())
            {
                colorRow[ColorTable.Defs.Columns.Value] = value.ToString();
                //OnPropertyChanged(id);
                OnPropertyChanged(nameof(Color));
            }
        }

        private bool GetIsEnabled()
        {
            return (bool)colorRow[ColorTable.Defs.Columns.Enabled];
        }

        private void SetIsEnabled(bool value)
        {
            if (value != GetIsEnabled())
            {
                colorRow[ColorTable.Defs.Columns.Enabled] = value;
                //OnPropertyChanged(id);
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
        #endregion
    }
}
