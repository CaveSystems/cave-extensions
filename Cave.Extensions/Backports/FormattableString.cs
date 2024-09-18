#pragma warning disable IDE0130
#pragma warning disable CS1591
#if (NETSTANDARD1_0_OR_GREATER && ! NETSTANDARD1_3_OR_GREATER) || ((NET20_OR_GREATER && !NET46_OR_GREATER) && ! NETCOREAPP2_0_OR_GREATER)

using System.Globalization;

namespace System;

public abstract class FormattableString : IFormattable
{
    public static string Invariant(FormattableString formattable)
    {
        if (formattable == null)
        {
            throw new ArgumentNullException(nameof(formattable));
        }

        return formattable.ToString(CultureInfo.InvariantCulture);
    }

    public abstract string Format { get; }
    public abstract int ArgumentCount { get; }

    string IFormattable.ToString(string ignored, IFormatProvider formatProvider) => ToString(formatProvider);

    public abstract object[] GetArguments();

    public abstract object GetArgument(int index);

    public abstract string ToString(IFormatProvider formatProvider);

    public override string ToString() => ToString(CultureInfo.CurrentCulture);
}

#endif
