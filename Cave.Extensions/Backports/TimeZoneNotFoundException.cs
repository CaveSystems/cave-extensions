#if NET20
#pragma warning disable SA1600 // No comments for backports
#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure
#nullable disable

using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System;

[Serializable]
[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
public class TimeZoneNotFoundException : Exception
{
    public TimeZoneNotFoundException(string message) : base(message) { }

    public TimeZoneNotFoundException(string message, Exception innerException) : base(message, innerException) { }

    protected TimeZoneNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public TimeZoneNotFoundException() { }
}

#endif
