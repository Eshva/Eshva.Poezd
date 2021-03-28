#region Usings

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Matching nothing queue name matcher.
  /// </summary>
  [ExcludeFromCodeCoverage]
  internal sealed class MatchingNothingQueueNameMatcher : IQueueNameMatcher
  {
    /// <inheritdoc />
    public bool DoesMatch(string queueName, string queueNamePattern) => false;
  }
}
