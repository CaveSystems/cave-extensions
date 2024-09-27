#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130
#pragma warning disable CS9113

#if NET20_OR_GREATER || (NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class MaybeNullWhenAttribute(bool result) : Attribute { }

#endif
