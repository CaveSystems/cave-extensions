#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20 || NET35 || NETSTANDARD10

using System.Diagnostics;

namespace System.Threading.Tasks;

[DebuggerDisplay("ShouldExitCurrentIteration = {ShouldExitCurrentIteration}")]
public class ParallelLoopState
{
    #region Properties

    public bool IsExceptional { get; private set; }

    public bool IsStopped { get; private set; }

    public bool ShouldExitCurrentIteration { get; private set; }

    internal bool StopByAnySource => IsExceptional || IsStopped || ShouldExitCurrentIteration;

    #endregion Properties

    #region Members

    public void Break() => ShouldExitCurrentIteration = true;

    public void Stop() => IsStopped = true;

    internal void SetException() => IsExceptional = true;

    #endregion Members
}

#endif
