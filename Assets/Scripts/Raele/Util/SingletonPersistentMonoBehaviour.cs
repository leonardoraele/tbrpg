using System;
using UnityEngine;

namespace Raele.Util
{
    public abstract class SingletonPersistentMonoBehaviour<T> : MonoBehaviour where T : SingletonPersistentMonoBehaviour<T>
    {
        public static T Instance
            => SingletonPersistentMonoBehaviour<T>.s_instance
                = SingletonPersistentMonoBehaviour<T>.s_instance
                ?? UnityEventListener.Instance.gameObject.AddComponent<T>();

        private static T s_instance;

        protected virtual void Awake()
        {
            s_instance = s_instance ?? (T) this;

            if (SingletonPersistentMonoBehaviour<T>.Instance.As(out T i) != this)
            {
                this.enabled = false;
                throw new Exception($"Found two instances of the supposedly singleton component of type {typeof(T).Name} in the scene, in object '{i.gameObject.name}' and '{this.gameObject.name}'. I\'m disabling the component in the later object so that only one exists.");
            }

            DontDestroyOnLoad(SingletonPersistentMonoBehaviour<T>.Instance.gameObject);
        }

        /// <summary>
        /// OnDestroy method to clear singleton association
        /// </summary>
        protected virtual void OnDestroy()
            => SingletonPersistentMonoBehaviour<T>.Instance
                .Then(() => SingletonPersistentMonoBehaviour<T>.Instance.gameObject == this.gameObject)
                .Then(() => SingletonPersistentMonoBehaviour<T>.s_instance = null);
    }
}
