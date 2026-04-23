#if NET20 || NET35 || NET40
#pragma warning disable SA1600 // No comments for backports
#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class CallerFilePathAttribute : Attribute { }

#endif
