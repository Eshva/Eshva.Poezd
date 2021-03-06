#region Usings

using System;
using Eshva.Poezd.Core.Common;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests.Tools
{
  public class TestClock : IClock
  {
    public TestClock(DateTimeOffset time)
    {
      _time = time;
    }

    public DateTimeOffset GetNowUtc() => _time;

    private readonly DateTimeOffset _time;
  }
}
