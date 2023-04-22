using System;
using System.Collections;
using System.Collections.Generic;

namespace Cave.Collections;

/// <summary>Gets an <see cref="IEnumerator" /> implementation for simple integer counting.</summary>
public class CountEnumerator : IEnumerator<int>, IEnumerator
{
    #region Fields

    readonly Counter counter;

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="CountEnumerator" /> class.</summary>
    /// <param name="counter">The <see cref="Counter" /> used to create values to be enumerated.</param>
    public CountEnumerator(Counter counter) => this.counter = counter;

    /// <summary>Initializes a new instance of the <see cref="CountEnumerator" /> class.</summary>
    /// <param name="start">The first value to be enumerated.</param>
    public CountEnumerator(int start)
        : this(new Counter(start)) { }

    /// <summary>Initializes a new instance of the <see cref="CountEnumerator" /> class.</summary>
    /// <param name="start">The first value to be enumerated.</param>
    /// <param name="count">The value count to be enumerated.</param>
    public CountEnumerator(int start, int count)
        : this(new Counter(start, count)) { }

    #endregion

    #region IEnumerator<int> Members

    /// <summary>Frees all used resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Gets the the curent value.</summary>
    object IEnumerator.Current => counter.Current;

    /// <summary>Moves to the next value.</summary>
    /// <returns></returns>
    public bool MoveNext() => counter.MoveNext();

    /// <summary>Resets the <see cref="CountEnumerator" />.</summary>
    public void Reset() => counter.Reset();

    /// <summary>Gets the the curent value.</summary>
    public int Current => counter.Current;

    #endregion

    #region Members

    /// <summary>Releases the unmanaged resources used by this instance and optionally releases the managed resources.</summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing) { }

    #endregion
}
