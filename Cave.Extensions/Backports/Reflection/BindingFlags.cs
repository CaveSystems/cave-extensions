#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD1_5_OR_GREATER)

#pragma warning disable CS1591

namespace System.Reflection;

public enum BindingFlags
{
    Default = 0x0,
    IgnoreCase = 0x1,
    DeclaredOnly = 0x2,
    Instance = 0x4,
    Static = 0x8,
    Public = 0x10,
    NonPublic = 0x20,
    FlattenHierarchy = 0x40,
    InvokeMethod = 0x100,
    CreateInstance = 0x200,
    GetField = 0x400,
    SetField = 0x800,
    GetProperty = 0x1000,
    SetProperty = 0x2000,
    PutDispProperty = 0x4000,
    PutRefDispProperty = 0x8000,
    ExactBinding = 0x10000,
    SuppressChangeType = 0x20000,
    OptionalParamBinding = 0x40000,
    IgnoreReturn = 0x1000000
}

#endif
