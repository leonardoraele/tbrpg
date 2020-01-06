using System;
using UnityEngine;

namespace Raele.Util
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (SingletonMonoBehaviour<T>.Instance.As(out T i) != null)
            {
                this.enabled = false;
                throw new Exception($"Found two instances of the supposedly singleton component of type {typeof(T).Name} in the scene, in object '{i.gameObject.name}' and '{this.gameObject.name}'. I\'m disabling the component in the later object so that only one exists.");
            }

            SingletonMonoBehaviour<T>.Instance = (T) this;
        }

        /// <summary>
        /// OnDestroy method to clear singleton association
        /// </summary>
        protected virtual void OnDestroy()
            => SingletonMonoBehaviour<T>.Instance
                .Then(() => SingletonMonoBehaviour<T>.Instance = null);
    }
}
