#region Usings

using System;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Eshva.Poezd.Core.Common
{
  [ExcludeFromCodeCoverage]
  public abstract class PoezdException : ApplicationException
  {
    /// <summary>
    /// Constructs a new instance of Poezd exception.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    protected PoezdException(string message) : base(message) { }

    /// <summary>
    /// Constructs a new instance of Poezd exception.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="innerException">
    /// The cause of this exception.
    /// </param>
    protected PoezdException(string message, Exception innerException) : base(message, innerException) { }
  }
}
