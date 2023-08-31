#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#pragma warning disable CS1591

namespace System.Reflection;

public static class PropertyInfoExtension
{
#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD1_5_OR_GREATER)

    public static MethodInfo GetGetMethod(this PropertyInfo propertyInfo) => propertyInfo.GetMethod;

    public static MethodInfo GetSetMethod(this PropertyInfo propertyInfo) => propertyInfo.SetMethod;

#endif

#if (NET20_OR_GREATER && !NET45_OR_GREATER)

    public static void SetValue(this PropertyInfo propertyInfo, object obj, object value) => propertyInfo.SetValue(obj, value, null);

    public static object GetValue(this PropertyInfo propertyInfo, object obj) => propertyInfo.GetValue(obj, null);

#endif
}
