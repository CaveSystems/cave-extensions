#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20 || NET35

namespace System.Threading.Tasks;

[Flags]
public enum TaskCreationOptions
{
    None = 0x0,
    PreferFairness = 0x1,
    LongRunning = 0x2
}

#endif
