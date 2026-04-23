using System;

namespace Cave;

/// <summary>
/// Provides an abstract base class for hash algorithm implementations removing the default GetHashCode and Equals implementations to prevent misuse. A hash
/// code is mutable during computation and should not be compared with other instances.
/// </summary>
public abstract class CaveHashAlgorithm : CaveHashAlgorithmBase
{
    #region Public Methods

    /// <summary>HashCode is mutable during computation and should not be compared with other instances.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>Always returns false.</returns>
    [Obsolete("Does not return a valid result! -> This method returns always false!")]
    public new bool Equals(object? obj) => false;

    /// <summary>Serves as the default hash function and returns the reference hash code for the current object.</summary>
    /// <returns>A hash code for the current object.</returns>
    [Obsolete("Does not return a valid hash code! -> This method returns the object.GetHashCode() result.")]
    public new int GetHashCode() => base.GetHashCode();

    #endregion Public Methods
}
