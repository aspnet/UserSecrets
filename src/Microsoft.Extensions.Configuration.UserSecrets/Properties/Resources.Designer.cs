// <auto-generated />
namespace Microsoft.Extensions.Configuration.UserSecrets
{
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    internal static class Resources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Microsoft.Extensions.Configuration.UserSecrets.Resources", typeof(Resources).GetTypeInfo().Assembly);

        /// <summary>
        /// Invalid character '{0}' found in 'userSecretsId' value at index '{1}'.
        /// </summary>
        internal static string Error_Invalid_Character_In_UserSecrets_Id
        {
            get { return GetString("Error_Invalid_Character_In_UserSecrets_Id"); }
        }

        /// <summary>
        /// Invalid character '{0}' found in 'userSecretsId' value at index '{1}'.
        /// </summary>
        internal static string FormatError_Invalid_Character_In_UserSecrets_Id(object p0, object p1)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_Invalid_Character_In_UserSecrets_Id"), p0, p1);
        }

        /// <summary>
        /// Unable to locate load the user secrets identifier file '{0}'.
        /// </summary>
        internal static string Error_Missing_Identifer_File
        {
            get { return GetString("Error_Missing_Identifer_File"); }
        }

        /// <summary>
        /// Unable to locate load the user secrets identifier file '{0}'.
        /// </summary>
        internal static string FormatError_Missing_Identifer_File(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_Missing_Identifer_File"), p0);
        }

        /// <summary>
        /// Missing 'userSecretsId' in '{0}'.
        /// </summary>
        internal static string Error_Missing_UserSecretId_In_Json
        {
            get { return GetString("Error_Missing_UserSecretId_In_Json"); }
        }

        /// <summary>
        /// Missing 'userSecretsId' in '{0}'.
        /// </summary>
        internal static string FormatError_Missing_UserSecretId_In_Json(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_Missing_UserSecretId_In_Json"), p0);
        }

        /// <summary>
        /// This platform cannot identify the entry assembly.
        /// </summary>
        internal static string Error_EntryAssembly_NotAvailable
        {
            get { return GetString("Error_EntryAssembly_NotAvailable"); }
        }

        /// <summary>
        /// This platform cannot identify the entry assembly.
        /// </summary>
        internal static string FormatError_EntryAssembly_NotAvailable()
        {
            return GetString("Error_EntryAssembly_NotAvailable");
        }

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
