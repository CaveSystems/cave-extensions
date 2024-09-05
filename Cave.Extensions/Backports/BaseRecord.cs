using System;
using System.Reflection;
using System.Text;

namespace Cave;

/// <summary>Basic record with equality backport using reflection for net20 and net35, empty record for all other frameworks</summary>
public record BaseRecord : IEquatable<BaseRecord>
{
#if NET20_OR_GREATER && !NET40_OR_GREATER
    FieldInfo[] Fields => EqualityContract.GetFields(BindingFlags.NonPublic | BindingFlags.Public);

    /// <inheritdoc/>
    public virtual bool Equals(BaseRecord other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (EqualityContract != other.EqualityContract) return false;
        foreach (var field in Fields)
        {
            if (!Equals(field.GetValue(this), field.GetValue(other))) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hasher = DefaultHashingFunction.Create();
        foreach (var field in Fields)
        {
            hasher.Add(field.GetValue(this));
        }
        return hasher.ToHashCode();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("{ ");
        var first = true;
        foreach (var field in Fields)
        {
            if (first) first = false; else sb.Append(", ");
            sb.Append('"');
            sb.Append(field.Name);
            sb.Append("\": \"");
            sb.Append(field.GetValue(this));
            sb.Append('"');
        }
        sb.Append('}');
        return sb.ToString();
    }
#endif
}
