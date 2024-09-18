#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20_OR_GREATER || NETSTANDARD2_0

namespace System;

public static partial class BackportedExtensions
{
    public static bool StartsWith(this string text, char c) => (text.Length > 1) && text[0].Equals(c);

    public static bool EndsWith(this string text, char c) => (text.Length > 1) && text[^1].Equals(c);
}

#endif
