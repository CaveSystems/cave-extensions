#if NET20 || NET35
#pragma warning disable SA1600 // No comments for backports
#pragma warning disable SA1602 // No comments for backports
#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Threading.Tasks;

[Flags]
public enum TaskCreationOptions
{
    None = 0x0,
    PreferFairness = 0x1,
    LongRunning = 0x2
}

#endif
