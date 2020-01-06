using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Raele.Util
{
    public static class R
    {
        public static T Identity<T>(T t) => t;
        public static void Hollow() {}
        public static R As<R>(object t) where R : class => t as R;

        public static bool Not(bool b) => !b;

        public static bool NotDefault<T>(T t) => t != null && !t.Equals(default(T));
        public static bool NotEmpty<T>(IEnumerable<T> ts) => ts.Any();

        public static int Sum(int a, int b) => a + b;
        public static float Sum(float a, float b) => a + b;
        public static float Sum(int a, float b) => a + b;
        public static float Sum(float a, int b) => a + b;

        public static void Invoke(Action action) => action.Invoke();
        public static T Invoke<T>(Func<T> func) => func.Invoke();
        public static void Quiet() {}

        public static void SafeInvoke(Action action) // Alias for Try
            => R.Try(action);

        public static void Try(Action action)
        {
            try {
                action();
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }
    }
}