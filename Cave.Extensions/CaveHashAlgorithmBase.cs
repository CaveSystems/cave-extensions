using System.Security.Cryptography;

namespace Cave;

/// <summary>
/// Provides a base class for implementing custom hash algorithms.
/// </summary>
public abstract class CaveHashAlgorithmBase : HashAlgorithm
{
    /// <summary>
    /// Serves as the default hash function and returns the reference hash code for the current object.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>HashCode is mutable during computation and should not be compared with other instances.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>Always returns false.</returns>
    public override bool Equals(object? obj) => false;
}
