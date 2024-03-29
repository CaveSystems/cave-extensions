﻿using System;

namespace Cave.Reflection;

/// <summary>SoftwareFlags for the Assembly.</summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class AssemblySoftwareFlagsAttribute : Attribute
{
    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="AssemblySoftwareFlagsAttribute" /> class.</summary>
    /// <param name="flags">The flags.</param>
    public AssemblySoftwareFlagsAttribute(SoftwareFlags flags) => Flags = flags;

    #endregion

    #region Properties

    /// <summary>Gets the SoftwareFlags.</summary>
    public SoftwareFlags Flags { get; }

    #endregion
}
