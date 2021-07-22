using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Cave.Reflection;

namespace Cave
{
    /// <summary>Gets extended version info for the specified file (replacement for FileVersionInfo).</summary>
    public struct AssemblyVersionInfo : IEquatable<AssemblyVersionInfo>
    {
        #region static implementation

        /// <summary>Checks two <see cref="AssemblyVersionInfo" /> for equality.</summary>
        /// <param name="v1">first version info.</param>
        /// <param name="v2">second version info.</param>
        /// <returns>true if versions are equal.</returns>
        public static bool operator ==(AssemblyVersionInfo v1, AssemblyVersionInfo v2) => v1.Equals(v2);

        /// <summary>Checks two <see cref="AssemblyVersionInfo" /> for inequality.</summary>
        /// <param name="v1">first version info.</param>
        /// <param name="v2">second version info.</param>
        /// <returns>true if versions are not equal.</returns>
        public static bool operator !=(AssemblyVersionInfo v1, AssemblyVersionInfo v2) => !v1.Equals(v2);

        static object programAssemblyVersionInfo;

        /// <summary>Gets the <see cref="AssemblyVersionInfo" /> for the current entry assembly.</summary>
        public static AssemblyVersionInfo Program
        {
            get
            {
                if (programAssemblyVersionInfo == null)
                {
                    var a = MainAssembly.Get();
                    if (a == null)
                    {
                        throw new InvalidOperationException("AppDomain inaccessible!");
                    }

                    programAssemblyVersionInfo = FromAssembly(a);
                }

                return (AssemblyVersionInfo)programAssemblyVersionInfo;
            }
        }

        /// <summary>Gets the <see cref="AssemblyVersionInfo" /> for the specified FileName.</summary>
        /// <param name="fileName">file name of the assembly.</param>
        /// <returns>the assembly version info.</returns>
        public static AssemblyVersionInfo FromAssemblyFile(string fileName) => FromAssembly(Assembly.LoadFile(fileName));

        /// <summary>Gets the <see cref="AssemblyVersionInfo" /> for the specified AssemblyName.</summary>
        /// <param name="assemblyName">name of the assembly.</param>
        /// <returns>the assembly version info.</returns>
        public static AssemblyVersionInfo FromAssemblyName(AssemblyName assemblyName) => FromAssembly(Assembly.Load(assemblyName));

        /// <summary>Gets the <see cref="AssemblyVersionInfo" /> for the specified Assembly.</summary>
        /// <param name="assembly">the assembly.</param>
        /// <returns>the assembly version info.</returns>
        public static AssemblyVersionInfo FromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            AssemblyVersionInfo i = default;
            var assemblyName = assembly.GetName();

            #region get assembly attributes

            foreach (Attribute attribute in assembly.GetCustomAttributes(false))
            {
                {
                    if (attribute is AssemblyCompanyAttribute a)
                    {
                        i.Company = a.Company;
                        continue;
                    }
                }
                {
                    if (attribute is AssemblyConfigurationAttribute a)
                    {
                        i.Configuration = a.Configuration;
                        continue;
                    }
                }
                {
                    if (attribute is AssemblyCopyrightAttribute a)
                    {
                        i.Copyright = a.Copyright;
                        continue;
                    }
                }
                {
                    if (attribute is AssemblyDescriptionAttribute a)
                    {
                        i.Description = a.Description;
                        continue;
                    }
                }
                {
                    if (attribute is AssemblyFileVersionAttribute a)
                    {
                        i.FileVersion = new Version(a.Version);
                        continue;
                    }
                }
                {
                    if (attribute is AssemblyInformationalVersionAttribute a)
                    {
                        i.InformalVersion = SemanticVersion.TryParse(a.InformationalVersion);
                        continue;
                    }
                }
                {
                    if (attribute is AssemblyProductAttribute a)
                    {
                        i.Product = a.Product;
                        continue;
                    }
                }
                {
                    if (attribute is AssemblyTitleAttribute a)
                    {
                        i.Title = a.Title;
                        continue;
                    }
                }
                {
                    if (attribute is AssemblyTrademarkAttribute a)
                    {
                        i.Trademark = a.Trademark;
                        continue;
                    }
                }
                {
                    if (attribute is GuidAttribute a)
                    {
                        i.Guid = new Guid(a.Value);
                        continue;
                    }
                }
                {
                    if (attribute is AssemblyUpdateUriAttribute a)
                    {
                        i.UpdateURI = a.URI;
                        continue;
                    }
                }
                {
                    if (attribute is AssemblySoftwareFlagsAttribute a)
                    {
                        i.SoftwareFlags = a.Flags;
                        continue;
                    }
                }
                {
                    if (attribute is AssemblySetupVersionAttribute a)
                    {
                        i.SetupVersion = a.SetupVersion;
                        continue;
                    }
                }
                {
                    if (attribute is AssemblySetupPackageAttribute a)
                    {
                        i.SetupPackage = a.SetupPackage;
                    }
                }
            }

            #endregion

            // get assembly name properties
            {
                i.AssemblyVersion = assemblyName.Version;
                i.Culture = assemblyName.CultureInfo;
                i.PublicKey = assemblyName.GetPublicKey();
                i.PublicKeyToken = assemblyName.GetPublicKeyToken().ToHexString();
            }
            return i;
        }

        #endregion

        #region fields

        /// <summary>Dataset ID. This is only used when reading/writing at a database.</summary>
        public long ID;

        /// <summary>The File Version.</summary>
        public Version FileVersion;

        /// <summary>The Assembly / Product Version.</summary>
        public Version AssemblyVersion;

        /// <summary>The Assembly display version.</summary>
        public SemanticVersion InformalVersion;

        /// <summary>The Setup Version.</summary>
        public Version SetupVersion;

        /// <summary>The Setup Package Name.</summary>
        public string SetupPackage;

        /// <summary>The SoftwareFlags.</summary>
        public SoftwareFlags SoftwareFlags;

        /// <summary>The title.</summary>
        public string Title;

        /// <summary>The product name.</summary>
        public string Product;

        /// <summary>The product description.</summary>
        public string Description;

        /// <summary>Name of the company.</summary>
        public string Company;

        /// <summary>Compiler configuration of the program.</summary>
        public string Configuration;

        /// <summary>The Assemblies' copyright.</summary>
        public string Copyright;

        /// <summary>The Assemblies' trademark.</summary>
        public string Trademark;

        /// <summary>The Assemblies' Culture LCID.</summary>
        public int CultureID;

        /// <summary>The Assemblies' full PublicKey.</summary>
        public byte[] PublicKey;

        /// <summary>The Assemblies' PublicKeyToken.</summary>
        public string PublicKeyToken;

        /// <summary>The Assemblies' Guid.</summary>
        public Guid Guid;

        /// <summary>The Assemblies' Update URI.</summary>
        public Uri UpdateURI;

        #endregion

        /// <summary>Gets or sets the Assemblies' CultureInfo.</summary>
        public CultureInfo Culture
        {
            get => new(CultureID);
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                CultureID = value.LCID;
            }
        }

        /// <summary>Gets the release date.</summary>
        /// <value>The release date.</value>
        public DateTime ReleaseDate
        {
            get
            {
                try
                {
                    return new DateTime(FileVersion.Major, FileVersion.Minor / 100, FileVersion.Minor % 100, FileVersion.Build / 100, FileVersion.Build % 100,
                        0, DateTimeKind.Utc);
                }
                catch
                {
                    return new DateTime(0);
                }
            }
        }

        /// <summary>Gets a latestversion instance for the current assembly (this populates only fields present at this instance).</summary>
        /// <returns>the latest version.</returns>
        public LatestVersion ToLatestVersion() =>
            new()
            {
                AssemblyVersion = AssemblyVersion,
                FileVersion = FileVersion,
                ReleaseDate = ReleaseDate,
                SoftwareName = Title,
                UpdateURI = UpdateURI,
                SetupPackage = SetupPackage,
                SetupVersion = SetupVersion,
                SoftwareFlags = SoftwareFlags
            };

        /// <summary>Gets the string describing this instance.</summary>
        /// <returns>Product name and informal version.</returns>
        public override string ToString() => Product + " " + InformalVersion;

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <param name="format">The format.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        /// <exception cref="NotSupportedException">Invalid format.</exception>
        public string ToString(string format)
        {
            var result = new StringBuilder();
            switch (format)
            {
                case "X":
                    foreach (var field in typeof(AssemblyVersionInfo).GetFields())
                    {
                        result.Append(field.Name);
                        result.Append(": ");
                        result.Append(field.GetValue(this));
                        result.AppendLine();
                    }

                    break;
                default: throw new NotSupportedException();
            }

            return result.ToString();
        }

        /// <summary>Gets a hashcode for this instance.</summary>
        /// <returns>the hash code.</returns>
        public override int GetHashCode() => Guid.GetHashCode();

        /// <summary>Checks this instance for equality with another one.</summary>
        /// <param name="obj">object to check for.</param>
        /// <returns>true if equal.</returns>
        public override bool Equals(object obj) => obj is AssemblyVersionInfo avi && Equals(avi);

        /// <summary>Checks this instance for equality with another one.</summary>
        /// <param name="other">Other version info to test.</param>
        /// <returns>true if equal.</returns>
        public bool Equals(AssemblyVersionInfo other)
        {
            var fields = typeof(AssemblyVersionInfo).GetFields(BindingFlags.Public);
            foreach (var field in fields)
            {
                if (field.GetValue(this) != field.GetValue(other))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
