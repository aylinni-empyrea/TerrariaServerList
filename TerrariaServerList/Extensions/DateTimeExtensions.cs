using System;

namespace Penny.Extensions
{
  public static class DateTimeExtensions
  {
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static double ToEpochSeconds(this DateTime time)
      => (time - Epoch).TotalSeconds;

    public static DateTime FromEpochSeconds(this double time)
      => Epoch.AddSeconds(Convert.ToDouble(time));

    public static double ToEpochMilliseconds(this DateTime time)
      => (time - Epoch).TotalMilliseconds;

    public static DateTime FromEpochMilliseconds(this double time)
      => Epoch.AddMilliseconds(Convert.ToDouble(time));
  }
}