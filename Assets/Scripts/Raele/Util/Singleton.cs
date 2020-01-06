using System;

namespace Raele.Util
{
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static T m_instance;
        public static T Instance => m_instance
            ?? (m_instance = typeof(T).GetConstructor(new Type[0]).Invoke(new object[0]) as T);
    }
}
