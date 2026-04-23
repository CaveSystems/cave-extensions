#if NET20_OR_GREATER || (NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
#pragma warning disable SA1600 // No comments for backports
#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable CS9113 // Parameter 'result' is not used in the attribute constructor.

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class MaybeNullWhenAttribute(bool result) : Attribute { }

#endif
