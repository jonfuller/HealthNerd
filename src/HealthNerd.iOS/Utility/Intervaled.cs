using NodaTime;

namespace HealthNerd.iOS.Utility
{
    public class Intervaled<T>
    {
        public Interval Interval { get; }
        public T Value { get; }


        public Intervaled(T value, Interval interval)
        {
            Value = value;
            Interval = interval;
        }
    }

    public static class Intervaled
    {
        public static Intervaled<T> Create<T>(T value, Interval interval)
        {
            return new Intervaled<T>(value, interval);
        }
    }
}