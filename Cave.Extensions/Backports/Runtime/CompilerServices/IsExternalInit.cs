#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if !NET5_0_OR_GREATER

// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE file in
// the project root for more information.

using System.ComponentModel;

namespace System.Runtime.CompilerServices;

/// <summary>Reserved to be used by the compiler for tracking metadata. This class should not be used by developers in source code.</summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class IsExternalInit { }

#endif
