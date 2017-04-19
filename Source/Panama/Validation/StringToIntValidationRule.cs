using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Restless.App.Panama.Resources;

namespace Restless.App.Panama
{
    /// <summary>
    /// Provides a custom validation rule for string to integer conversion
    /// </summary>
    public class StringToIntValidationRule : ValidationRule
    {
        /// <summary>
        /// Validates that the supplied value can be converted to an integer,
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="cultureInfo">Not used.</param>
        /// <returns>The validation result</returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int i;
            if (int.TryParse(value.ToString(), out i))
            {
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, Strings.ValidationOperationIntegerNeeded);
        }
    }
}
