using System;

public static class TimeUtility
{
    static readonly DateTime m_UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);

    public static DateTime serverTime
    {
        set
        {
            gap = value - DateTime.Now;
        }
    }

    public static TimeSpan gap
    {
        get;
        private set;
    }

    public static DateTime currentServerTime
    {
        get
        {
            return DateTime.Now + gap;
        }
    }

    public static double currentServerTimeUnixEpoch
    {
        get
        {
            return ToUnixEpoch(currentServerTime);
        }
    }

    public static DateTime ToDateTime(double value)
    {
        return m_UnixEpoch.AddSeconds(value);
    }

    public static double ToUnixEpoch(DateTime value)
    {
        return (value - m_UnixEpoch).TotalSeconds;
    }
}
