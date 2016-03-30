using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;

namespace MSCorp.AdventureWorks.Web
{
    /// <summary>
    /// Responsible for loading settings from config
    /// </summary>
    public static class SettingLoader
    {
        /// <summary>
        /// Loads a setting with the specified key.
        /// </summary>
        public static Setting Load(string key)
        {
            return new Setting(key, CloudConfigurationManager.GetSetting(key));
        }
    }


    /// <summary>
    /// Setting
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Setting" /> class.
        /// </summary>
        public Setting(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>        
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Returns a integer representation of this configuration setting
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        public int ToInt()
        {
            int intValue;
            if (int.TryParse(Value, out intValue))
            {
                return intValue;
            }
            string error = string.Format(CultureInfo.CurrentCulture, "The configuration setting '{0}' with value '{1}' cannot be converted to an int",
                Key, Value);
            throw new InvalidOperationException(error);
        }

        /// <summary>
        /// Returns an enum of the specified type for this configuration setting
        /// </summary>
        public T ToEnum<T>() where T : struct
        {
            T enumValue;
            if (Enum.TryParse(Value, out enumValue))
            {
                return enumValue;
            }

            string error = string.Format(CultureInfo.CurrentCulture, "The configuration setting '{0}' with value '{1}' cannot be converted to an enum of type {2}",
                Key, Value, typeof(T));
            throw new InvalidOperationException(error);
        }

        /// <summary>
        /// Returns a URI from the setting
        /// </summary>
        public Uri ToUri()
        {
            return new Uri(ToString());
        }

        /// <summary>
        /// Converts the setting to a boolean value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool")]
        public bool ToBool()
        {
            bool outValue;
            if (bool.TryParse(Value, out outValue))
            {
                return outValue;
            }

            string error = string.Format(CultureInfo.CurrentCulture, "The configuration setting '{0}' with value '{1}' cannot be converted to boolean value", Key, Value);
            throw new InvalidOperationException(error);
        }
    }

}
