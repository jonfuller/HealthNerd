﻿using System;

namespace HealthNerd.Utility
{
    public static class FunctionalExtensions
    {
        public static T2 Then<T1, T2>(this T1 x, Func<T1, T2> f)
        {
            return f(x);
        }

        public static void Then<T1>(this T1 x, Action<T1> f)
        {
            f(x);
        }
    }
}