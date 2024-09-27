#if NET20 || NET35
#nullable disable
#pragma warning disable IDE0130
#pragma warning disable CS1591
#pragma warning disable CS8601

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System;

[Serializable]
[SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
public class AggregateException : Exception
{
    #region Fields

    readonly IList<Exception> exceptions;

    #endregion Fields

    #region Constructors

    public AggregateException()
        : base("One ore more exceptions occured.") => exceptions = new Exception[0];

    public AggregateException(params Exception[] innerExceptions)
        : this("One ore more exceptions occured.", innerExceptions) => exceptions = innerExceptions;

    public AggregateException(IEnumerable<Exception> innerExceptions)
        : this("One ore more exceptions occured.", innerExceptions) => exceptions = innerExceptions.ToList();

    public AggregateException(string msg)
        : base(msg) => exceptions = new Exception[0];

    public AggregateException(string msg, Exception innerException)
        : base(msg) => exceptions = new[] { innerException };

    public AggregateException(string msg, params Exception[] innerExceptions)
        : base(msg, (innerExceptions != null) && (innerExceptions.Length > 0) ? innerExceptions[0] : null) => exceptions = innerExceptions;

    public AggregateException(string msg, IEnumerable<Exception> innerExceptions)
        : this(msg, innerExceptions.ToArray()) { }

    protected AggregateException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }

    #endregion Constructors

    #region Properties

    public ReadOnlyCollectionWrapper<Exception> InnerExceptions => new(exceptions);

    #endregion Properties

    #region Overrides

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);

    #endregion Overrides
}

#endif
