namespace Cave;

/// <summary>Provides a filter function definition for <see cref="PropertyEnumerator" /> and <see cref="PropertyValueEnumerator" />.</summary>
/// <param name="property">Property to be filtered.</param>
/// <returns>Returns a value indicating whether the property shall be filtered (true) or not (false).</returns>
public delegate bool PropertyDataFilter(PropertyData property);
