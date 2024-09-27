using System;

namespace Cave.Reflection;

/// <summary>License Number Attribute for the Assembly.</summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class AssemblySetupPackageAttribute : Attribute
{
    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="AssemblySetupPackageAttribute"/> class.</summary>
    /// <param name="package">The package.</param>
    public AssemblySetupPackageAttribute(string package) => SetupPackage = package;

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the license number.</summary>
    public string SetupPackage { get; }

    #endregion Public Properties
}
