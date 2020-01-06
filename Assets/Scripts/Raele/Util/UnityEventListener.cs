using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Threading;

namespace Raele.Util
{
    public class UnityEventListener : MonoBehaviour
    {
		/// <summary>
		/// Eagerly initializes the instance.
		/// </summary>
        public static UnityEventListener Instance
			=> s_instance = s_instance
				?? GameObject.FindObjectOfType<UnityEventListener>()
				?? (Application.isEditor && EditorApplication.isPlayingOrWillChangePlaymode)
					.Then(() => new GameObject($"{typeof(UnityEventListener).FullName}").AddComponent<UnityEventListener>())
					.OtherwiseThrow($"Cannot access {typeof(UnityEventListener).Name} instance in editor mode.");

		public static bool IsInUnityRunLoop => Thread.CurrentThread == s_unity_runloop_thread;

		public static Coroutine StartCoroutine(Func<IEnumerator> coroutineFunction)
			=> coroutineFunction().Then(UnityEventListener.Instance.StartCoroutine);

		private static UnityEventListener s_instance;
		private static Thread s_unity_runloop_thread;

		public event Action OnUpdateEvent;
		public event Action OnGUIEvent;

        private void Awake()
		{
			s_unity_runloop_thread = Thread.CurrentThread;
			GameObject.DontDestroyOnLoad(this.gameObject);
		}

		private void Update()
			=> this.OnUpdateEvent?.SafeInvoke();
		
		private void OnGUI()
			=> this.OnGUIEvent?.SafeInvoke();

		private void OnValidate()
			=> (Application.isEditor && !EditorApplication.isPlayingOrWillChangePlaymode)
				.Then(() => Debug.LogWarning($"{typeof(UnityEventListener).Name} is attached to an object in a scene; it was supposed to be only accessed through the 'Instance' static accessor."));

        private void OnApplicationQuit()
			// TODO For some weird reason Destroy is not destroying this game object
			=> GameObject.Destroy(this.gameObject);
    }
}
