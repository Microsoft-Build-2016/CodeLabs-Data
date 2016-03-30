using System;
using System.Reflection;

namespace Common
{
    /// <summary>
    /// Parses the version number of the currently executing assembly and provides properties for display on forms
    /// </summary>
    public class AssemblyVersion
    {
        private readonly Version _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyVersion"/> class.
        /// </summary>
        public AssemblyVersion()
        {
            _version = Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// Gets the full version
        /// </summary>
        /// <value>The full version.</value>
        /// <remarks>Returns the version in the format Major.Minor.Build.Revision</remarks>
        public string FullVersion
        {
            get { return "{0}.{1}.{2}.{3}".FormatWith(_version.Major, _version.Minor, _version.Build, _version.Revision); }
        }
    }
}