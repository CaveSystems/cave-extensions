#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
#pragma warning disable SA1600 // No comments for backports
#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

using System.Collections.Generic;
using System.Linq;

namespace System.Collections;

public class ArrayList : List<object>, IList
{
    public ArrayList() { }

    public ArrayList(int capacity) : base(capacity) { }

    public ArrayList(ICollection collection) : base(collection.Cast<object>()) { }
}
#endif
