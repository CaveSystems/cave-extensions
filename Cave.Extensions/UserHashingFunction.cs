using System;

namespace Cave.CodeGen;

/// <summary>
/// Provides the single instance used to combine all hashes at generated code.
/// </summary>
public static class UserHashingFunction
{
    /// <summary>
    /// Provides the function used to create the <see cref="IUserHashingFunction"/> used to combine hashes.
    /// </summary>
    public static Func<IUserHashingFunction> Create { get; set; } = () => new XxHash32();
}
